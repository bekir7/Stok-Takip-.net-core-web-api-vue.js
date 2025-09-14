<template>
  <div class="products-page">
    <div class="page-header">
      <h2>Ürün Yönetimi</h2>
      <el-button type="primary" @click="openCreateDialog">
        <el-icon><Plus /></el-icon>
        Yeni Ürün Ekle
      </el-button>
    </div>

    <!-- Search and Filters -->
    <el-card class="filter-card">
      <el-row :gutter="20">
        <el-col :span="8">
          <el-input
            v-model="searchQuery"
            placeholder="Ürün ara..."
            @input="handleSearch"
            clearable
          >
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
        </el-col>
        <el-col :span="6">
          <el-select v-model="selectedCategory" placeholder="Kategori" clearable>
            <el-option label="Tümü" value="" />
            <el-option
              v-for="category in categories"
              :key="category"
              :label="category"
              :value="category"
            />
          </el-select>
        </el-col>
        <el-col :span="6">
          <el-select v-model="stockFilter" placeholder="Stok Durumu" clearable>
            <el-option label="Tümü" value="" />
            <el-option label="Düşük Stok" value="low" />
            <el-option label="Stokta Yok" value="empty" />
          </el-select>
        </el-col>
        <el-col :span="4">
          <el-button @click="resetFilters">Filtreleri Temizle</el-button>
        </el-col>
      </el-row>
    </el-card>

    <!-- Products Table -->
    <el-card>
      <el-table :data="filteredProducts" v-loading="loading" style="width: 100%">
        <el-table-column prop="productCode" label="Ürün Kodu" width="120" />
        <el-table-column prop="productName" label="Ürün Adı" />
        <el-table-column prop="category" label="Kategori" width="120" />
        <el-table-column prop="brand" label="Marka" width="120" />
        <el-table-column prop="stockQuantity" label="Stok" width="80" align="center" />
        <el-table-column prop="minStockLevel" label="Min. Stok" width="80" align="center" />
        <el-table-column prop="unitPrice" label="Birim Fiyat" width="100" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.unitPrice) }}
          </template>
        </el-table-column>
        <el-table-column prop="unit" label="Birim" width="80" align="center" />
        <el-table-column label="Durum" width="100" align="center">
          <template #default="scope">
            <el-tag 
              :type="scope.row.stockQuantity <= scope.row.minStockLevel ? 'danger' : 'success'"
              size="small"
            >
              {{ scope.row.stockQuantity <= scope.row.minStockLevel ? 'Düşük' : 'Normal' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="İşlemler" width="200" align="center">
          <template #default="scope">
            <el-button size="small" @click="editProduct(scope.row)">Düzenle</el-button>
            <el-button size="small" type="warning" @click="updateStock(scope.row)">Stok Güncelle</el-button>
            <el-button size="small" type="danger" @click="deleteProduct(scope.row.productId)">Sil</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- Create/Edit Product Dialog -->
    <el-dialog
      v-model="showCreateDialog"
      :title="editingProduct ? 'Ürün Düzenle' : 'Yeni Ürün Ekle'"
      width="900px"
      :close-on-click-modal="false"
      :close-on-press-escape="false"
    >
      <el-form :model="productForm" :rules="productRules" ref="productFormRef" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Ürün Adı" prop="productName">
              <el-input v-model="productForm.productName" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Ürün Kodu" prop="productCode">
              <el-input v-model="productForm.productCode" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-form-item label="Açıklama">
          <el-input type="textarea" v-model="productForm.description" />
        </el-form-item>
        
        <el-row :gutter="20">
          <el-col :span="10">
            <el-form-item label="Birim Fiyat" prop="unitPrice">
              <el-input 
                v-model.number="productForm.unitPrice" 
                type="number"
                :step="0.01"
                :min="0"
                style="width: 100%; font-size: 18px;" 
              />
            </el-form-item>
          </el-col>
          <el-col :span="10">
            <el-form-item label="Stok Miktarı" prop="stockQuantity">
              <el-input 
                v-model.number="productForm.stockQuantity" 
                type="number"
                :min="0" 
                :step="1"
                style="width: 100%; font-size: 18px;" 
              />
            </el-form-item>
          </el-col>
          <el-col :span="10">
            <el-form-item label="Min. Stok" prop="minStockLevel">
              <el-input 
                v-model.number="productForm.minStockLevel" 
                type="number"
                :min="0" 
                :step="1"
                style="width: 100%; font-size: 18px;" 
              />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="Birim">
              <el-select v-model="productForm.unit" placeholder="Seçiniz">
                <el-option label="Adet" value="Adet" />
                <el-option label="Kg" value="Kg" />
                <el-option label="Litre" value="Litre" />
                <el-option label="Metre" value="Metre" />
                <el-option label="Paket" value="Paket" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="Kategori">
              <el-input v-model="productForm.category" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="Marka">
              <el-input v-model="productForm.brand" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-form-item label="Resim URL">
          <el-input v-model="productForm.imageUrl" />
        </el-form-item>
        
        <el-form-item v-if="editingProduct" label="Aktif">
          <el-switch v-model="productForm.isActive" />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="showCreateDialog = false">İptal</el-button>
        <el-button type="primary" @click="saveProduct" :loading="saving">
          {{ editingProduct ? 'Güncelle' : 'Kaydet' }}
        </el-button>
      </template>
    </el-dialog>

    <!-- Update Stock Dialog -->
    <el-dialog
      v-model="showStockDialog"
      title="Stok Güncelle"
      width="400px"
      :close-on-click-modal="false"
      :close-on-press-escape="false"
    >
      <el-form :model="stockForm" :rules="stockRules" ref="stockFormRef" label-width="100px">
        <el-form-item label="Ürün">
          <el-input v-model="stockForm.productName" readonly />
        </el-form-item>
        <el-form-item label="Mevcut Stok">
          <el-input 
            v-model.number="stockForm.currentStock" 
            readonly 
            type="number"
            style="width: 100%" 
          />
        </el-form-item>
        <el-form-item label="Yeni Miktar" prop="newQuantity">
          <el-input 
            v-model.number="stockForm.newQuantity" 
            type="number"
            :min="0"
            style="width: 100%" 
          />
        </el-form-item>
        <el-form-item label="Açıklama">
          <el-input type="textarea" v-model="stockForm.description" />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="showStockDialog = false">İptal</el-button>
        <el-button type="primary" @click="saveStockUpdate" :loading="saving">Güncelle</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { productService } from '../services/productService'
import { ElMessage, ElMessageBox } from 'element-plus'

export default {
  name: 'Products',
  setup() {
    const loading = ref(false)
    const saving = ref(false)
    const products = ref([])
    const searchQuery = ref('')
    const selectedCategory = ref('')
    const stockFilter = ref('')
    const showCreateDialog = ref(false)
    const showStockDialog = ref(false)
    const editingProduct = ref(null)
    const productFormRef = ref()
    const stockFormRef = ref()

    const productForm = ref({
      productName: '',
      productCode: '',
      description: '',
      unitPrice: 0,
      stockQuantity: 0,
      minStockLevel: 0,
      unit: '',
      category: '',
      brand: '',
      imageUrl: '',
      isActive: true
    })

    const stockForm = ref({
      productId: null,
      productName: '',
      currentStock: 0,
      newQuantity: 0,
      description: ''
    })

    const productRules = {
      productName: [
        { required: true, message: 'Ürün adı gereklidir', trigger: 'blur' }
      ],
      unitPrice: [
        { required: true, message: 'Birim fiyat gereklidir', trigger: 'blur' }
      ],
      stockQuantity: [
        { required: true, message: 'Stok miktarı gereklidir', trigger: 'blur' }
      ],
      minStockLevel: [
        { required: true, message: 'Minimum stok seviyesi gereklidir', trigger: 'blur' }
      ]
    }

    const stockRules = {
      newQuantity: [
        { required: true, message: 'Yeni miktar gereklidir', trigger: 'blur' }
      ]
    }

    const categories = computed(() => {
      const cats = [...new Set(products.value.map(p => p.category).filter(Boolean))]
      return cats.sort()
    })

    const filteredProducts = computed(() => {
      let filtered = products.value

      if (searchQuery.value) {
        const query = searchQuery.value.toLowerCase()
        filtered = filtered.filter(p => 
          p.productName.toLowerCase().includes(query) ||
          p.productCode?.toLowerCase().includes(query) ||
          p.description?.toLowerCase().includes(query)
        )
      }

      if (selectedCategory.value) {
        filtered = filtered.filter(p => p.category === selectedCategory.value)
      }

      if (stockFilter.value === 'low') {
        filtered = filtered.filter(p => p.stockQuantity <= p.minStockLevel)
      } else if (stockFilter.value === 'empty') {
        filtered = filtered.filter(p => p.stockQuantity === 0)
      }

      return filtered
    })

    const loadProducts = async () => {
      loading.value = true
      try {
        products.value = await productService.getAllProducts()
      } catch (error) {
        ElMessage.error('Ürünler yüklenirken hata oluştu')
      } finally {
        loading.value = false
      }
    }

    const handleSearch = () => {
      // Search is handled by computed property
    }

    const resetFilters = () => {
      searchQuery.value = ''
      selectedCategory.value = ''
      stockFilter.value = ''
    }

    const openCreateDialog = () => {
      resetProductForm()
      // Clear form validation
      if (productFormRef.value) {
        productFormRef.value.clearValidate()
      }
      showCreateDialog.value = true
    }

    const editProduct = (product) => {
      editingProduct.value = product
      productForm.value = { ...product }
      // Clear form validation
      if (productFormRef.value) {
        productFormRef.value.clearValidate()
      }
      showCreateDialog.value = true
    }

    const updateStock = (product) => {
      stockForm.value = {
        productId: product.productId,
        productName: product.productName,
        currentStock: product.stockQuantity,
        newQuantity: product.stockQuantity,
        description: ''
      }
      showStockDialog.value = true
    }

    const saveProduct = async () => {
      if (!productFormRef.value) return
      
      const valid = await productFormRef.value.validate()
      if (!valid) return

      saving.value = true
      try {
        // Create product data without isActive for create, with isActive for update
        const productData = { ...productForm.value }
        
        if (editingProduct.value) {
          await productService.updateProduct(editingProduct.value.productId, productData)
          ElMessage.success('Ürün güncellendi')
        } else {
          // Remove isActive from create request since backend doesn't expect it
          delete productData.isActive
          await productService.createProduct(productData)
          ElMessage.success('Ürün eklendi')
        }
        
        showCreateDialog.value = false
        resetProductForm()
        await loadProducts()
      } catch (error) {
        console.error('Product save error:', error)
        ElMessage.error('Ürün kaydedilirken hata oluştu: ' + (error.response?.data?.message || error.message))
      } finally {
        saving.value = false
      }
    }

    const saveStockUpdate = async () => {
      if (!stockFormRef.value) return
      
      const valid = await stockFormRef.value.validate()
      if (!valid) return

      saving.value = true
      try {
        const quantityChange = stockForm.value.newQuantity - stockForm.value.currentStock
        await productService.updateStock(
          stockForm.value.productId,
          quantityChange,
          stockForm.value.description
        )
        
        ElMessage.success('Stok güncellendi')
        showStockDialog.value = false
        await loadProducts()
      } catch (error) {
        ElMessage.error('Stok güncellenirken hata oluştu')
      } finally {
        saving.value = false
      }
    }

    const deleteProduct = async (productId) => {
      try {
        await ElMessageBox.confirm(
          'Bu ürünü silmek istediğinizden emin misiniz?',
          'Uyarı',
          {
            confirmButtonText: 'Evet',
            cancelButtonText: 'Hayır',
            type: 'warning',
          }
        )

        await productService.deleteProduct(productId)
        ElMessage.success('Ürün silindi')
        await loadProducts()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('Ürün silinirken hata oluştu')
        }
      }
    }

    const resetProductForm = () => {
      productForm.value = {
        productName: '',
        productCode: '',
        description: '',
        unitPrice: 0,
        stockQuantity: 0,
        minStockLevel: 0,
        unit: '',
        category: '',
        brand: '',
        imageUrl: '',
        isActive: true
      }
      editingProduct.value = null
    }

    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('tr-TR', {
        style: 'currency',
        currency: 'TRY'
      }).format(amount)
    }

    onMounted(() => {
      loadProducts()
    })

    return {
      loading,
      saving,
      products,
      searchQuery,
      selectedCategory,
      stockFilter,
      showCreateDialog,
      showStockDialog,
      editingProduct,
      productForm,
      stockForm,
      productFormRef,
      stockFormRef,
      productRules,
      stockRules,
      categories,
      filteredProducts,
      loadProducts,
      handleSearch,
      resetFilters,
      openCreateDialog,
      editProduct,
      updateStock,
      saveProduct,
      saveStockUpdate,
      deleteProduct,
      formatCurrency
    }
  }
}
</script>

<style scoped>
.products-page {
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

.el-card {
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

/* Fix for input-number display issues */
.el-input-number {
  width: 100% !important;
  position: relative !important;
}

.el-input-number .el-input__inner {
  text-align: left !important;
  padding-right: 40px !important;
  padding-left: 12px !important;
  width: 100% !important;
  box-sizing: border-box !important;
  font-size: 14px !important;
  color: #606266 !important;
  background-color: #fff !important;
  border: 1px solid #dcdfe6 !important;
  border-radius: 4px !important;
}

.el-input-number .el-input__wrapper {
  padding-right: 40px !important;
  width: 100% !important;
}

.el-input-number .el-input-number__increase,
.el-input-number .el-input-number__decrease {
  width: 18px !important;
  height: 18px !important;
  position: absolute !important;
  right: 1px !important;
  top: 1px !important;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  font-size: 10px !important;
  background: #f5f7fa !important;
  color: #606266 !important;
  border: none !important;
  cursor: pointer !important;
  user-select: none !important;
}

.el-input-number .el-input-number__increase {
  border-top-right-radius: 3px !important;
  border-bottom-right-radius: 0 !important;
  border-bottom: 1px solid #dcdfe6 !important;
}

.el-input-number .el-input-number__decrease {
  border-top-right-radius: 0 !important;
  border-bottom-right-radius: 3px !important;
  top: 19px !important;
}

.el-input-number .el-input-number__increase:hover,
.el-input-number .el-input-number__decrease:hover {
  background: #e6f7ff !important;
  color: #409eff !important;
}

/* Ensure input value is visible */
.el-input-number input {
  color: #606266 !important;
  font-weight: normal !important;
}
</style>
