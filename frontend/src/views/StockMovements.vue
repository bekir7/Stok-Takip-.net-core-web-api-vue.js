<template>
  <div class="stock-movements-page">
    <div class="page-header">
      <h2>Stok Hareketleri</h2>
    </div>

    <!-- Filters -->
    <el-card class="filter-card">
      <el-row :gutter="20">
        <el-col :span="6">
          <el-input
            v-model="searchQuery"
            placeholder="Stok hareketi ara..."
            @input="handleSearch"
            clearable
          >
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
        </el-col>
        <el-col :span="4">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            range-separator="to"
            start-placeholder="Başlangıç"
            end-placeholder="Bitiş"
            format="DD.MM.YYYY"
            value-format="YYYY-MM-DD"
            @change="handleDateFilter"
          />
        </el-col>
        <el-col :span="4">
          <el-select v-model="movementTypeFilter" placeholder="Hareket Türü" clearable>
            <el-option label="Tümü" value="" />
            <el-option label="Giriş" value="In" />
            <el-option label="Çıkış" value="Out" />
            <el-option label="Düzeltme" value="Adjustment" />
          </el-select>
        </el-col>
        <el-col :span="4">
          <el-select v-model="productFilter" placeholder="Ürün" clearable>
            <el-option label="Tümü" value="" />
            <el-option
              v-for="product in products"
              :key="product.productId"
              :label="product.productName"
              :value="product.productId"
            />
          </el-select>
        </el-col>
        <el-col :span="6">
          <el-button @click="resetFilters">Filtreleri Temizle</el-button>
        </el-col>
      </el-row>
    </el-card>

    <!-- Stock Movements Table -->
    <el-card>
      <el-table :data="filteredStockMovements" v-loading="loading" style="width: 100%">
        <el-table-column prop="movementDate" label="Tarih" width="120">
          <template #default="scope">
            {{ formatDate(scope.row.movementDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="productName" label="Ürün" />
        <el-table-column prop="movementType" label="Hareket Türü" width="120" align="center">
          <template #default="scope">
            <el-tag 
              :type="getMovementTypeColor(scope.row.movementType)"
              size="small"
            >
              {{ getMovementTypeLabel(scope.row.movementType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="quantity" label="Miktar" width="100" align="center">
          <template #default="scope">
            <span :class="scope.row.movementType === 'In' ? 'positive-quantity' : 'negative-quantity'">
              {{ scope.row.movementType === 'In' ? '+' : '-' }}{{ scope.row.quantity }}
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="unitPrice" label="Birim Fiyat" width="120" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.unitPrice) }}
          </template>
        </el-table-column>
        <el-table-column prop="totalAmount" label="Toplam Tutar" width="120" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.totalAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="description" label="Açıklama" />
        <el-table-column prop="referenceNumber" label="Referans No" width="120" />
        <el-table-column label="İşlemler" width="100" align="center">
          <template #default="scope">
            <el-button size="small" type="danger" @click="deleteStockMovement(scope.row.stockMovementId)">Sil</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- Summary Cards -->
    <el-row :gutter="20" style="margin-top: 20px;">
      <el-col :span="6">
        <el-card class="summary-card">
          <div class="summary-content">
            <div class="summary-icon in">
              <el-icon><ArrowDown /></el-icon>
            </div>
            <div class="summary-info">
              <h3>{{ summary.totalIn }}</h3>
              <p>Toplam Giriş</p>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="summary-card">
          <div class="summary-content">
            <div class="summary-icon out">
              <el-icon><ArrowUp /></el-icon>
            </div>
            <div class="summary-info">
              <h3>{{ summary.totalOut }}</h3>
              <p>Toplam Çıkış</p>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="summary-card">
          <div class="summary-content">
            <div class="summary-icon adjustment">
              <el-icon><Refresh /></el-icon>
            </div>
            <div class="summary-info">
              <h3>{{ summary.totalAdjustment }}</h3>
              <p>Toplam Düzeltme</p>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="summary-card">
          <div class="summary-content">
            <div class="summary-icon total">
              <el-icon><TrendCharts /></el-icon>
            </div>
            <div class="summary-info">
              <h3>{{ summary.totalMovements }}</h3>
              <p>Toplam Hareket</p>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { stockMovementService } from '../services/stockMovementService'
import { productService } from '../services/productService'
import { ElMessage, ElMessageBox } from 'element-plus'
import dayjs from 'dayjs'

export default {
  name: 'StockMovements',
  setup() {
    const loading = ref(false)
    const stockMovements = ref([])
    const products = ref([])
    const searchQuery = ref('')
    const dateRange = ref(null)
    const movementTypeFilter = ref('')
    const productFilter = ref('')

    const summary = computed(() => {
      const movements = filteredStockMovements.value
      return {
        totalIn: movements.filter(m => m.movementType === 'In').reduce((sum, m) => sum + m.quantity, 0),
        totalOut: movements.filter(m => m.movementType === 'Out').reduce((sum, m) => sum + m.quantity, 0),
        totalAdjustment: movements.filter(m => m.movementType === 'Adjustment').length,
        totalMovements: movements.length
      }
    })

    const filteredStockMovements = computed(() => {
      let filtered = stockMovements.value

      if (searchQuery.value) {
        const query = searchQuery.value.toLowerCase()
        filtered = filtered.filter(m => 
          m.productName?.toLowerCase().includes(query) ||
          m.description?.toLowerCase().includes(query) ||
          m.referenceNumber?.toLowerCase().includes(query)
        )
      }

      if (dateRange.value && dateRange.value.length === 2) {
        const [start, end] = dateRange.value
        filtered = filtered.filter(m => {
          const movementDate = dayjs(m.movementDate)
          return movementDate.isAfter(dayjs(start).subtract(1, 'day')) && movementDate.isBefore(dayjs(end).add(1, 'day'))
        })
      }

      if (movementTypeFilter.value) {
        filtered = filtered.filter(m => m.movementType === movementTypeFilter.value)
      }

      if (productFilter.value) {
        filtered = filtered.filter(m => m.productId === productFilter.value)
      }

      return filtered.sort((a, b) => new Date(b.movementDate) - new Date(a.movementDate))
    })

    const loadData = async () => {
      loading.value = true
      try {
        const [movementsData, productsData] = await Promise.all([
          stockMovementService.getAllStockMovements(),
          productService.getAllProducts()
        ])
        
        stockMovements.value = movementsData
        products.value = productsData.filter(p => p.isActive)
      } catch (error) {
        ElMessage.error('Veriler yüklenirken hata oluştu')
      } finally {
        loading.value = false
      }
    }

    const handleSearch = () => {
      // Search is handled by computed property
    }

    const handleDateFilter = () => {
      // Date filter is handled by computed property
    }

    const resetFilters = () => {
      searchQuery.value = ''
      dateRange.value = null
      movementTypeFilter.value = ''
      productFilter.value = ''
    }

    const deleteStockMovement = async (stockMovementId) => {
      try {
        await ElMessageBox.confirm(
          'Bu stok hareketini silmek istediğinizden emin misiniz?',
          'Uyarı',
          {
            confirmButtonText: 'Evet',
            cancelButtonText: 'Hayır',
            type: 'warning',
          }
        )

        await stockMovementService.deleteStockMovement(stockMovementId)
        ElMessage.success('Stok hareketi silindi')
        await loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('Stok hareketi silinirken hata oluştu')
        }
      }
    }

    const getMovementTypeLabel = (type) => {
      const labels = {
        'In': 'Giriş',
        'Out': 'Çıkış',
        'Adjustment': 'Düzeltme'
      }
      return labels[type] || type
    }

    const getMovementTypeColor = (type) => {
      const colors = {
        'In': 'success',
        'Out': 'danger',
        'Adjustment': 'warning'
      }
      return colors[type] || 'info'
    }

    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('tr-TR', {
        style: 'currency',
        currency: 'TRY'
      }).format(amount)
    }

    const formatDate = (date) => {
      return dayjs(date).format('DD.MM.YYYY')
    }

    onMounted(() => {
      loadData()
    })

    return {
      loading,
      stockMovements,
      products,
      searchQuery,
      dateRange,
      movementTypeFilter,
      productFilter,
      summary,
      filteredStockMovements,
      loadData,
      handleSearch,
      handleDateFilter,
      resetFilters,
      deleteStockMovement,
      getMovementTypeLabel,
      getMovementTypeColor,
      formatCurrency,
      formatDate
    }
  }
}
</script>

<style scoped>
.stock-movements-page {
  max-width: 1200px;
  margin: 0 auto;
}

.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.page-header h2 {
  margin: 0;
  color: #303133;
}

.filter-card {
  margin-bottom: 20px;
}

.positive-quantity {
  color: #67c23a;
  font-weight: 600;
}

.negative-quantity {
  color: #f56c6c;
  font-weight: 600;
}

.summary-card {
  margin-bottom: 20px;
}

.summary-content {
  display: flex;
  align-items: center;
  gap: 15px;
}

.summary-icon {
  width: 50px;
  height: 50px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  color: white;
}

.summary-icon.in {
  background: linear-gradient(135deg, #67c23a 0%, #85ce61 100%);
}

.summary-icon.out {
  background: linear-gradient(135deg, #f56c6c 0%, #f78989 100%);
}

.summary-icon.adjustment {
  background: linear-gradient(135deg, #e6a23c 0%, #ebb563 100%);
}

.summary-icon.total {
  background: linear-gradient(135deg, #409eff 0%, #66b1ff 100%);
}

.summary-info h3 {
  margin: 0;
  font-size: 1.8rem;
  font-weight: 700;
  color: #303133;
}

.summary-info p {
  margin: 0;
  color: #909399;
  font-size: 0.9rem;
}

.el-card {
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}
</style>
