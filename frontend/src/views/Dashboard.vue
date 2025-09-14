<template>
  <div class="dashboard">
    <el-row :gutter="20">
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon products">
              <el-icon><Goods /></el-icon>
            </div>
            <div class="stat-info">
              <h3>{{ stats.totalProducts }}</h3>
              <p>Toplam Ürün</p>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon customers">
              <el-icon><User /></el-icon>
            </div>
            <div class="stat-info">
              <h3>{{ stats.totalCustomers }}</h3>
              <p>Toplam Müşteri</p>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon suppliers">
              <el-icon><OfficeBuilding /></el-icon>
            </div>
            <div class="stat-info">
              <h3>{{ stats.totalSuppliers }}</h3>
              <p>Toplam Tedarikçi</p>
            </div>
          </div>
        </el-card>
      </el-col>
      
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-content">
            <div class="stat-icon low-stock">
              <el-icon><Warning /></el-icon>
            </div>
            <div class="stat-info">
              <h3>{{ stats.lowStockProducts }}</h3>
              <p>Düşük Stok</p>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="20" style="margin-top: 20px;">
      <el-col :span="12">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>Düşük Stoklu Ürünler</span>
              <el-button type="primary" size="small" @click="$router.push('/products')">
                Tümünü Gör
              </el-button>
            </div>
          </template>
          <el-table :data="lowStockProducts" v-loading="loading">
            <el-table-column prop="productName" label="Ürün Adı" />
            <el-table-column prop="stockQuantity" label="Mevcut Stok" width="100" />
            <el-table-column prop="minStockLevel" label="Min. Stok" width="100" />
            <el-table-column label="Durum" width="80">
              <template #default="scope">
                <el-tag type="danger" size="small">Kritik</el-tag>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>
      
      <el-col :span="12">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>Son Satışlar</span>
              <el-button type="primary" size="small" @click="$router.push('/sales')">
                Tümünü Gör
              </el-button>
            </div>
          </template>
          <el-table :data="recentSales" v-loading="loading">
            <el-table-column prop="saleNumber" label="Satış No" width="100" />
            <el-table-column prop="customerName" label="Müşteri" />
            <el-table-column prop="totalAmount" label="Tutar" width="100">
              <template #default="scope">
                {{ formatCurrency(scope.row.totalAmount) }}
              </template>
            </el-table-column>
            <el-table-column prop="saleDate" label="Tarih" width="120">
              <template #default="scope">
                {{ formatDate(scope.row.saleDate) }}
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import { productService } from '../services/productService'
import { customerService } from '../services/customerService'
import { supplierService } from '../services/supplierService'
import { saleService } from '../services/saleService'
import dayjs from 'dayjs'

export default {
  name: 'Dashboard',
  setup() {
    const loading = ref(false)
    const stats = ref({
      totalProducts: 0,
      totalCustomers: 0,
      totalSuppliers: 0,
      lowStockProducts: 0
    })
    const lowStockProducts = ref([])
    const recentSales = ref([])

    const loadDashboardData = async () => {
      loading.value = true
      try {
        // Load statistics
        const [products, customers, suppliers, lowStock] = await Promise.all([
          productService.getAllProducts(),
          customerService.getAllCustomers(),
          supplierService.getAllSuppliers(),
          productService.getLowStockProducts()
        ])

        stats.value = {
          totalProducts: products.length,
          totalCustomers: customers.length,
          totalSuppliers: suppliers.length,
          lowStockProducts: lowStock.length
        }

        lowStockProducts.value = lowStock.slice(0, 5) // Show only first 5

        // Load recent sales
        const sales = await saleService.getAllSales()
        recentSales.value = sales
          .sort((a, b) => new Date(b.saleDate) - new Date(a.saleDate))
          .slice(0, 5) // Show only first 5

      } catch (error) {
        console.error('Error loading dashboard data:', error)
      } finally {
        loading.value = false
      }
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
      loadDashboardData()
    })

    return {
      loading,
      stats,
      lowStockProducts,
      recentSales,
      formatCurrency,
      formatDate
    }
  }
}
</script>

<style scoped>
.dashboard {
  max-width: 1200px;
  margin: 0 auto;
}

.stat-card {
  margin-bottom: 20px;
}

.stat-content {
  display: flex;
  align-items: center;
  gap: 15px;
}

.stat-icon {
  width: 60px;
  height: 60px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  color: white;
}

.stat-icon.products {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.stat-icon.customers {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
}

.stat-icon.suppliers {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
}

.stat-icon.low-stock {
  background: linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%);
}

.stat-info h3 {
  margin: 0;
  font-size: 2rem;
  font-weight: 700;
  color: #303133;
}

.stat-info p {
  margin: 0;
  color: #909399;
  font-size: 0.9rem;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
