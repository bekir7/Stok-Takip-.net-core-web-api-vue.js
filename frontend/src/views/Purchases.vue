<template>
  <div class="purchases-page">
    <div class="page-header">
      <h2>Alış Yönetimi</h2>
      <el-button type="primary" @click="showCreateDialog = true">
        <el-icon><Plus /></el-icon>
        Yeni Alış
      </el-button>
    </div>

    <!-- Filters -->
    <el-card class="filter-card">
      <el-row :gutter="20">
        <el-col :span="6">
          <el-input
            v-model="searchQuery"
            placeholder="Alış ara..."
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

    <!-- Purchases Table -->
    <el-card>
      <el-table :data="filteredPurchases" v-loading="loading" style="width: 100%">
        <el-table-column prop="purchaseNumber" label="Alış No" width="120" />
        <el-table-column prop="supplierName" label="Tedarikçi" />
        <el-table-column prop="purchaseDate" label="Tarih" width="120">
          <template #default="scope">
            {{ formatDate(scope.row.purchaseDate) }}
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
            <el-button size="small" @click="viewPurchase(scope.row)">Görüntüle</el-button>
            <el-button size="small" type="danger" @click="deletePurchase(scope.row.purchaseId)">Sil</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- Create Purchase Dialog -->
    <el-dialog
      v-model="showCreateDialog"
      title="Yeni Alış"
      width="800px"
    >
      <el-form :model="purchaseForm" :rules="purchaseRules" ref="purchaseFormRef" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Tedarikçi" prop="supplierId">
              <el-select v-model="purchaseForm.supplierId" placeholder="Tedarikçi seçin" style="width: 100%">
                <el-option
                  v-for="supplier in suppliers"
                  :key="supplier.supplierId"
                  :label="supplier.supplierName"
                  :value="supplier.supplierId"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Alış Tarihi" prop="purchaseDate">
              <el-date-picker
                v-model="purchaseForm.purchaseDate"
                type="date"
                placeholder="Tarih seçin"
                format="DD.MM.YYYY"
                value-format="YYYY-MM-DD"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <!-- Purchase Items -->
        <el-divider content-position="left">Alış Kalemleri</el-divider>
        <div v-for="(item, index) in purchaseForm.purchaseItems" :key="index" class="purchase-item">
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
              <el-button type="danger" size="small" @click="removePurchaseItem(index)" :disabled="purchaseForm.purchaseItems.length === 1">
                <el-icon><Delete /></el-icon>
              </el-button>
            </el-col>
          </el-row>
        </div>
        
        <el-button type="primary" @click="addPurchaseItem">Ürün Ekle</el-button>

        <el-row :gutter="20" style="margin-top: 20px;">
          <el-col :span="8">
            <el-form-item label="İndirim">
              <el-input-number v-model="purchaseForm.discountAmount" :precision="2" :min="0" @change="calculateTotals" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="Ödeme Yöntemi">
              <el-select v-model="purchaseForm.paymentMethod" placeholder="Seçiniz">
                <el-option label="Nakit" value="Nakit" />
                <el-option label="Kredi Kartı" value="Kredi Kartı" />
                <el-option label="Havale" value="Havale" />
                <el-option label="Çek" value="Çek" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="Ödenmiş">
              <el-switch v-model="purchaseForm.isPaid" />
            </el-form-item>
          </el-col>
        </el-row>

        <el-form-item label="Notlar">
          <el-input type="textarea" v-model="purchaseForm.notes" />
        </el-form-item>

        <!-- Totals -->
        <el-divider content-position="left">Özet</el-divider>
        <el-row :gutter="20">
          <el-col :span="8">
            <strong>Ara Toplam: {{ formatCurrency(purchaseForm.subTotal) }}</strong>
          </el-col>
          <el-col :span="8">
            <strong>KDV: {{ formatCurrency(purchaseForm.taxAmount) }}</strong>
          </el-col>
          <el-col :span="8">
            <strong>Toplam: {{ formatCurrency(purchaseForm.totalAmount) }}</strong>
          </el-col>
        </el-row>
      </el-form>
      
      <template #footer>
        <el-button @click="showCreateDialog = false">İptal</el-button>
        <el-button type="primary" @click="savePurchase" :loading="saving">Kaydet</el-button>
      </template>
    </el-dialog>

    <!-- View Purchase Dialog -->
    <el-dialog
      v-model="showViewDialog"
      title="Alış Detayı"
      width="600px"
    >
      <div v-if="selectedPurchase">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="Alış No">{{ selectedPurchase.purchaseNumber }}</el-descriptions-item>
          <el-descriptions-item label="Tedarikçi">{{ selectedPurchase.supplierName }}</el-descriptions-item>
          <el-descriptions-item label="Tarih">{{ formatDate(selectedPurchase.purchaseDate) }}</el-descriptions-item>
          <el-descriptions-item label="Ödeme Durumu">
            <el-tag :type="selectedPurchase.isPaid ? 'success' : 'warning'">
              {{ selectedPurchase.isPaid ? 'Ödenmiş' : 'Ödenmemiş' }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="Ara Toplam">{{ formatCurrency(selectedPurchase.subTotal) }}</el-descriptions-item>
          <el-descriptions-item label="KDV">{{ formatCurrency(selectedPurchase.taxAmount) }}</el-descriptions-item>
          <el-descriptions-item label="İndirim">{{ formatCurrency(selectedPurchase.discountAmount) }}</el-descriptions-item>
          <el-descriptions-item label="Toplam">{{ formatCurrency(selectedPurchase.totalAmount) }}</el-descriptions-item>
        </el-descriptions>

        <el-divider content-position="left">Alış Kalemleri</el-divider>
        <el-table :data="selectedPurchase.purchaseItems" size="small">
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
import { purchaseService } from '../services/purchaseService'
import { supplierService } from '../services/supplierService'
import { productService } from '../services/productService'
import { ElMessage, ElMessageBox } from 'element-plus'
import dayjs from 'dayjs'

export default {
  name: 'Purchases',
  setup() {
    const loading = ref(false)
    const saving = ref(false)
    const purchases = ref([])
    const suppliers = ref([])
    const products = ref([])
    const searchQuery = ref('')
    const dateRange = ref(null)
    const paymentFilter = ref('')
    const showCreateDialog = ref(false)
    const showViewDialog = ref(false)
    const selectedPurchase = ref(null)
    const purchaseFormRef = ref()

    const purchaseForm = ref({
      supplierId: null,
      purchaseDate: dayjs().format('YYYY-MM-DD'),
      purchaseItems: [{
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

    const purchaseRules = {
      supplierId: [
        { required: true, message: 'Tedarikçi seçimi gereklidir', trigger: 'change' }
      ],
      purchaseDate: [
        { required: true, message: 'Alış tarihi gereklidir', trigger: 'change' }
      ]
    }

    const filteredPurchases = computed(() => {
      let filtered = purchases.value

      if (searchQuery.value) {
        const query = searchQuery.value.toLowerCase()
        filtered = filtered.filter(p => 
          p.purchaseNumber.toLowerCase().includes(query) ||
          p.supplierName?.toLowerCase().includes(query)
        )
      }

      if (dateRange.value && dateRange.value.length === 2) {
        const [start, end] = dateRange.value
        filtered = filtered.filter(p => {
          const purchaseDate = dayjs(p.purchaseDate)
          return purchaseDate.isAfter(dayjs(start).subtract(1, 'day')) && purchaseDate.isBefore(dayjs(end).add(1, 'day'))
        })
      }

      if (paymentFilter.value === 'paid') {
        filtered = filtered.filter(p => p.isPaid)
      } else if (paymentFilter.value === 'unpaid') {
        filtered = filtered.filter(p => !p.isPaid)
      }

      return filtered
    })

    const loadData = async () => {
      loading.value = true
      try {
        const [purchasesData, suppliersData, productsData] = await Promise.all([
          purchaseService.getAllPurchases(),
          supplierService.getAllSuppliers(),
          productService.getAllProducts()
        ])
        
        purchases.value = purchasesData
        suppliers.value = suppliersData.filter(s => s.isActive)
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

    const addPurchaseItem = () => {
      purchaseForm.value.purchaseItems.push({
        productId: null,
        quantity: 1,
        unitPrice: 0,
        discountRate: 0,
        totalAmount: 0
      })
    }

    const removePurchaseItem = (index) => {
      if (purchaseForm.value.purchaseItems.length > 1) {
        purchaseForm.value.purchaseItems.splice(index, 1)
        calculateTotals()
      }
    }

    const onProductChange = (index) => {
      const product = products.value.find(p => p.productId === purchaseForm.value.purchaseItems[index].productId)
      if (product) {
        purchaseForm.value.purchaseItems[index].unitPrice = product.unitPrice
        calculateItemTotal(index)
      }
    }

    const calculateItemTotal = (index) => {
      const item = purchaseForm.value.purchaseItems[index]
      const subtotal = item.quantity * item.unitPrice
      const discount = subtotal * (item.discountRate / 100)
      item.totalAmount = subtotal - discount
      calculateTotals()
    }

    const calculateTotals = () => {
      const subTotal = purchaseForm.value.purchaseItems.reduce((sum, item) => sum + item.totalAmount, 0)
      const discountAmount = purchaseForm.value.discountAmount || 0
      const afterDiscount = subTotal - discountAmount
      const taxAmount = afterDiscount * 0.18 // %18 KDV
      const totalAmount = afterDiscount + taxAmount

      purchaseForm.value.subTotal = subTotal
      purchaseForm.value.taxAmount = taxAmount
      purchaseForm.value.totalAmount = totalAmount
    }

    const savePurchase = async () => {
      if (!purchaseFormRef.value) return
      
      const valid = await purchaseFormRef.value.validate()
      if (!valid) return

      if (purchaseForm.value.purchaseItems.every(item => !item.productId)) {
        ElMessage.error('En az bir ürün seçmelisiniz')
        return
      }

      saving.value = true
      try {
        await purchaseService.createPurchase(purchaseForm.value)
        ElMessage.success('Alış kaydedildi')
        showCreateDialog.value = false
        resetPurchaseForm()
        await loadData()
      } catch (error) {
        ElMessage.error('Alış kaydedilirken hata oluştu')
      } finally {
        saving.value = false
      }
    }

    const viewPurchase = (purchase) => {
      selectedPurchase.value = purchase
      showViewDialog.value = true
    }

    const deletePurchase = async (purchaseId) => {
      try {
        await ElMessageBox.confirm(
          'Bu alışı silmek istediğinizden emin misiniz?',
          'Uyarı',
          {
            confirmButtonText: 'Evet',
            cancelButtonText: 'Hayır',
            type: 'warning',
          }
        )

        await purchaseService.deletePurchase(purchaseId)
        ElMessage.success('Alış silindi')
        await loadData()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('Alış silinirken hata oluştu')
        }
      }
    }

    const resetPurchaseForm = () => {
      purchaseForm.value = {
        supplierId: null,
        purchaseDate: dayjs().format('YYYY-MM-DD'),
        purchaseItems: [{
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
      purchases,
      suppliers,
      products,
      searchQuery,
      dateRange,
      paymentFilter,
      showCreateDialog,
      showViewDialog,
      selectedPurchase,
      purchaseForm,
      purchaseFormRef,
      purchaseRules,
      filteredPurchases,
      loadData,
      handleSearch,
      handleDateFilter,
      resetFilters,
      addPurchaseItem,
      removePurchaseItem,
      onProductChange,
      calculateItemTotal,
      calculateTotals,
      savePurchase,
      viewPurchase,
      deletePurchase,
      formatCurrency,
      formatDate
    }
  }
}
</script>

<style scoped>
.purchases-page {
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

.purchase-item {
  border: 1px solid #e4e7ed;
  padding: 10px;
  border-radius: 4px;
  margin-bottom: 10px;
}

.el-card {
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}
</style>
