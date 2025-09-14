<template>
  <div class="sales-page">
    <div class="page-header">
      <h2>Satış Yönetimi</h2>
      <el-button type="primary" @click="showCreateDialog = true">
        <el-icon><Plus /></el-icon>
        Yeni Satış
      </el-button>
    </div>

    <!-- Filters -->
    <el-card class="filter-card">
      <el-row :gutter="20">
        <el-col :span="6">
          <el-input
            v-model="searchQuery"
            placeholder="Satış ara..."
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
          <el-select v-model="paymentFilter" placeholder="Ödeme Durumu" clearable>
            <el-option label="Tümü" value="" />
            <el-option label="Ödenmiş" value="paid" />
            <el-option label="Ödenmemiş" value="unpaid" />
          </el-select>
        </el-col>
        <el-col :span="4">
          <el-button @click="resetFilters">Filtreleri Temizle</el-button>
        </el-col>
      </el-row>
    </el-card>

    <!-- Sales Table -->
    <el-card>
      <el-table :data="filteredSales" v-loading="loading" style="width: 100%">
        <el-table-column prop="saleNumber" label="Satış No" width="120" />
        <el-table-column prop="customerName" label="Müşteri" />
        <el-table-column prop="saleDate" label="Tarih" width="120">
          <template #default="scope">
            {{ formatDate(scope.row.saleDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="subTotal" label="Ara Toplam" width="120" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.subTotal) }}
          </template>
        </el-table-column>
        <el-table-column prop="taxAmount" label="KDV" width="100" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.taxAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="discountAmount" label="İndirim" width="100" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.discountAmount) }}
          </template>
        </el-table-column>
        <el-table-column prop="totalAmount" label="Toplam" width="120" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.totalAmount) }}
          </template>
        </el-table-column>
        <el-table-column label="Ödeme Durumu" width="120" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.isPaid ? 'success' : 'warning'" size="small">
              {{ scope.row.isPaid ? 'Ödenmiş' : 'Ödenmemiş' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="İşlemler" width="150" align="center">
          <template #default="scope">
            <el-button size="small" @click="viewSale(scope.row)">Görüntüle</el-button>
            <el-button size="small" type="danger" @click="deleteSale(scope.row.saleId)">Sil</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- Create Sale Dialog -->
    <el-dialog
      v-model="showCreateDialog"
      title="Yeni Satış"
      width="800px"
    >
      <el-form :model="saleForm" :rules="saleRules" ref="saleFormRef" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Müşteri" prop="customerId">
              <el-select v-model="saleForm.customerId" placeholder="Müşteri seçin" style="width: 100%">
                <el-option
                  v-for="customer in customers"
                  :key="customer.customerId"
                  :label="customer.customerName"
                  :value="customer.customerId"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Satış Tarihi" prop="saleDate">
              <el-date-picker
                v-model="saleForm.saleDate"
                type="date"
                placeholder="Tarih seçin"
                format="DD.MM.YYYY"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <!-- Sale Items -->
        <el-divider content-position="left">Satış Kalemleri</el-divider>
        <div v-for="(item, index) in saleForm.saleItems" :key="index" class="sale-item">
          <el-row :gutter="10" style="margin-bottom: 10px;">
            <el-col :span="8">
              <el-select v-model="item.productId" placeholder="Ürün seçin" @change="onProductChange(index)">
                <el-option
                  v-for="product in products"
                  :key="product.productId"
                  :label="product.productName"
                  :value="product.productId"
                />
              </el-select>
            </el-col>
            <el-col :span="4">
              <el-input-number v-model="item.quantity" :min="1" @change="calculateItemTotal(index)" />
            </el-col>
            <el-col :span="4">
              <el-input-number v-model="item.unitPrice" :precision="2" @change="calculateItemTotal(index)" />
            </el-col>
            <el-col :span="4">
              <el-input-number v-model="item.discountRate" :precision="2" :min="0" :max="100" @change="calculateItemTotal(index)" />
            </el-col>
            <el-col :span="3">
              <el-input v-model="item.totalAmount" readonly />
            </el-col>
            <el-col :span="1">
              <el-button type="danger" size="small" @click="removeSaleItem(index)" :disabled="saleForm.saleItems.length === 1">
                <el-icon><Delete /></el-icon>
              </el-button>
            </el-col>
          </el-row>
        </div>
        
        <el-button type="primary" @click="addSaleItem">Ürün Ekle</el-button>

        <el-row :gutter="20" style="margin-top: 20px;">
          <el-col :span="8">
            <el-form-item label="İndirim">
              <el-input-number v-model="saleForm.discountAmount" :precision="2" :min="0" @change="calculateTotals" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="Ödeme Yöntemi">
              <el-select v-model="saleForm.paymentMethod" placeholder="Seçiniz">
                <el-option label="Nakit" value="Nakit" />
                <el-option label="Kredi Kartı" value="Kredi Kartı" />
                <el-option label="Havale" value="Havale" />
                <el-option label="Çek" value="Çek" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="Ödenmiş">
              <el-switch v-model="saleForm.isPaid" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="Notlar">
          <el-input type="textarea" v-model="saleForm.notes" />
        </el-form-item>

        <!-- Totals -->
        <el-divider content-position="left">Özet</el-divider>
        <el-row :gutter="20">
          <el-col :span="8">
            <strong>Ara Toplam: {{ formatCurrency(saleForm.subTotal) }}</strong>
          </el-col>
          <el-col :span="8">
            <strong>KDV: {{ formatCurrency(saleForm.taxAmount) }}</strong>
          </el-col>
          <el-col :span="8">
            <strong>Toplam: {{ formatCurrency(saleForm.totalAmount) }}</strong>
          </el-col>
        </el-row>
      </el-form>
      
      <template #footer>
        <el-button @click="showCreateDialog = false">İptal</el-button>
        <el-button type="primary" @click="saveSale" :loading="saving">Kaydet</el-button>
      </template>
    </el-dialog>

    <!-- View Sale Dialog -->
    <el-dialog
      v-model="showViewDialog"
      title="Satış Detayı"
      width="600px"
    >
      <div v-if="selectedSale">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="Satış No">{{ selectedSale.saleNumber }}</el-descriptions-item>
          <el-descriptions-item label="Müşteri">{{ selectedSale.customerName }}</el-descriptions-item>
          <el-descriptions-item label="Tarih">{{ formatDate(selectedSale.saleDate) }}</el-descriptions-item>
          <el-descriptions-item label="Ödeme Durumu">
            <el-tag :type="selectedSale.isPaid ? 'success' : 'warning'">
              {{ selectedSale.isPaid ? 'Ödenmiş' : 'Ödenmemiş' }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="Ara Toplam">{{ formatCurrency(selectedSale.subTotal) }}</el-descriptions-item>
          <el-descriptions-item label="KDV">{{ formatCurrency(selectedSale.taxAmount) }}</el-descriptions-item>
          <el-descriptions-item label="İndirim">{{ formatCurrency(selectedSale.discountAmount) }}</el-descriptions-item>
          <el-descriptions-item label="Toplam">{{ formatCurrency(selectedSale.totalAmount) }}</el-descriptions-item>
        </el-descriptions>

        <el-divider content-position="left">Satış Kalemleri</el-divider>
        <el-table :data="selectedSale.saleItems" size="small">
          <el-table-column prop="productName" label="Ürün" />
          <el-table-column prop="quantity" label="Miktar" width="80" align="center" />
          <el-table-column prop="unitPrice" label="Birim Fiyat" width="100" align="right">
            <template #default="scope">
              {{ formatCurrency(scope.row.unitPrice) }}
            </template>
          </el-table-column>
          <el-table-column prop="totalAmount" label="Toplam" width="100" align="right">
            <template #default="scope">
              {{ formatCurrency(scope.row.totalAmount) }}
            </template>
          </el-table-column>
        </el-table>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { saleService } from '../services/saleService'
import { customerService } from '../services/customerService'
import { productService } from '../services/productService'
import { ElMessage, ElMessageBox } from 'element-plus'
import dayjs from 'dayjs'

export default {
  name: 'Sales',
  setup() {
    const loading = ref(false)
    const saving = ref(false)
    const sales = ref([])
    const customers = ref([])
    const products = ref([])
    const searchQuery = ref('')
    const dateRange = ref(null)
    const paymentFilter = ref('')
    const showCreateDialog = ref(false)
    const showViewDialog = ref(false)
    const selectedSale = ref(null)
    const saleFormRef = ref()

    const saleForm = ref({
      customerId: null,
      saleDate: dayjs().format('YYYY-MM-DD'),
      saleItems: [{
        productId: null,
        quantity: 1,
        unitPrice: 0,
        discountRate: 0,
        totalAmount: 0
      }],
      subTotal: 0,
      taxAmount: 0,
      discountAmount: 0,
      totalAmount: 0,
      notes: '',
      paymentMethod: '',
      isPaid: false
    })

    const saleRules = {
      customerId: [
        { required: true, message: 'Müşteri seçimi gereklidir', trigger: 'change' }
      ],
      saleDate: [
        { required: true, message: 'Satış tarihi gereklidir', trigger: 'change' }
      ]
    }

    const filteredSales = computed(() => {
      let filtered = sales.value

      if (searchQuery.value) {
        const query = searchQuery.value.toLowerCase()
        filtered = filtered.filter(s => 
          s.saleNumber.toLowerCase().includes(query) ||
          s.customerName?.toLowerCase().includes(query)
        )
      }

      if (dateRange.value && dateRange.value.length === 2) {
        const [start, end] = dateRange.value
        filtered = filtered.filter(s => {
          const saleDate = dayjs(s.saleDate)
          return saleDate.isAfter(dayjs(start).subtract(1, 'day')) && saleDate.isBefore(dayjs(end).add(1, 'day'))
        })
      }

      if (paymentFilter.value === 'paid') {
        filtered = filtered.filter(s => s.isPaid)
      } else if (paymentFilter.value === 'unpaid') {
        filtered = filtered.filter(s => !s.isPaid)
      }

      return filtered
    })

    const loadData = async () => {
      loading.value = true
      try {
        const [salesData, customersData, productsData] = await Promise.all([
          saleService.getAllSales(),
          customerService.getAllCustomers(),
          productService.getAllProducts()
        ])
        
        sales.value = salesData
        customers.value = customersData.filter(c => c.isActive)
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
      paymentFilter.value = ''
    }

    const addSaleItem = () => {
      saleForm.value.saleItems.push({
        productId: null,
        quantity: 1,
        unitPrice: 0,
        discountRate: 0,
        totalAmount: 0
      })
    }

    const removeSaleItem = (index) => {
      if (saleForm.value.saleItems.length > 1) {
        saleForm.value.saleItems.splice(index, 1)
        calculateTotals()
      }
    }

    const onProductChange = (index) => {
      const product = products.value.find(p => p.productId === saleForm.value.saleItems[index].productId)
      if (product) {
        saleForm.value.saleItems[index].unitPrice = product.unitPrice
        calculateItemTotal(index)
      }
    }

    const calculateItemTotal = (index) => {
      const item = saleForm.value.saleItems[index]
      const subtotal = item.quantity * item.unitPrice
      const discount = subtotal * (item.discountRate / 100)
      item.totalAmount = subtotal - discount
      calculateTotals()
    }

    const calculateTotals = () => {
      const subTotal = saleForm.value.saleItems.reduce((sum, item) => sum + item.totalAmount, 0)
      const discountAmount = saleForm.value.discountAmount || 0
      const afterDiscount = subTotal - discountAmount
      const taxAmount = afterDiscount * 0.18 // %18 KDV
      const totalAmount = afterDiscount + taxAmount

      saleForm.value.subTotal = subTotal
      saleForm.value.taxAmount = taxAmount
      saleForm.value.totalAmount = totalAmount
    }

    const saveSale = async () => {
      if (!saleFormRef.value) return
      
      const valid = await saleFormRef.value.validate()
      if (!valid) return

      if (saleForm.value.saleItems.every(item => !item.productId)) {
        ElMessage.error('En az bir ürün seçmelisiniz')
        return
      }

      saving.value = true
      try {
        await saleService.createSale(saleForm.value)
        ElMessage.success('Satış kaydedildi')
        showCreateDialog.value = false
        resetSaleForm()
        await loadData()
      } catch (error) {
        ElMessage.error('Satış kaydedilirken hata oluştu')
      } finally {
        saving.value = false
      }
    }

    const viewSale = (sale) => {
      selectedSale.value = sale
      showViewDialog.value = true
    }

    const deleteSale = async (saleId) => {
      try {
        await ElMessageBox.confirm(
          'Bu satışı silmek istediğinizden emin misiniz?',
          'Uyarı',
          {
            confirmButtonText: 'Evet',
            cancelButtonText: 'Hayır',
            type: 'warning',
          }
        )

        await saleService.deleteSale(saleId)
        ElMessage.success('Satış silindi')
        await loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('Satış silinirken hata oluştu')
        }
      }
    }

    const resetSaleForm = () => {
      saleForm.value = {
        customerId: null,
        saleDate: dayjs().format('YYYY-MM-DD'),
        saleItems: [{
          productId: null,
          quantity: 1,
          unitPrice: 0,
          discountRate: 0,
          totalAmount: 0
        }],
        subTotal: 0,
        taxAmount: 0,
        discountAmount: 0,
        totalAmount: 0,
        notes: '',
        paymentMethod: '',
        isPaid: false
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
      loadData()
    })

    return {
      loading,
      saving,
      sales,
      customers,
      products,
      searchQuery,
      dateRange,
      paymentFilter,
      showCreateDialog,
      showViewDialog,
      selectedSale,
      saleForm,
      saleFormRef,
      saleRules,
      filteredSales,
      loadData,
      handleSearch,
      handleDateFilter,
      resetFilters,
      addSaleItem,
      removeSaleItem,
      onProductChange,
      calculateItemTotal,
      calculateTotals,
      saveSale,
      viewSale,
      deleteSale,
      formatCurrency,
      formatDate
    }
  }
}
</script>

<style scoped>
.sales-page {
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

.sale-item {
  border: 1px solid #e4e7ed;
  padding: 10px;
  border-radius: 4px;
  margin-bottom: 10px;
}

.el-card {
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}
</style>
