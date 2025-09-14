<template>
  <div class="suppliers-page">
    <div class="page-header">
      <h2>Tedarikçi Yönetimi</h2>
      <el-button type="primary" @click="showCreateDialog = true">
        <el-icon><Plus /></el-icon>
        Yeni Tedarikçi Ekle
      </el-button>
    </div>

    <!-- Search -->
    <el-card class="filter-card">
      <el-row :gutter="20">
        <el-col :span="8">
          <el-input
            v-model="searchQuery"
            placeholder="Tedarikçi ara..."
            @input="handleSearch"
            clearable
          >
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
        </el-col>
        <el-col :span="4">
          <el-button @click="resetFilters">Filtreleri Temizle</el-button>
        </el-col>
      </el-row>
    </el-card>

    <!-- Suppliers Table -->
    <el-card>
      <el-table :data="filteredSuppliers" v-loading="loading" style="width: 100%">
        <el-table-column prop="supplierName" label="Tedarikçi Adı" />
        <el-table-column prop="companyName" label="Şirket" />
        <el-table-column prop="contactPerson" label="İletişim Kişisi" />
        <el-table-column prop="phoneNumber" label="Telefon" width="130" />
        <el-table-column prop="email" label="E-posta" />
        <el-table-column prop="currentBalance" label="Bakiye" width="120" align="right">
          <template #default="scope">
            <span :class="scope.row.currentBalance < 0 ? 'negative-balance' : 'positive-balance'">
              {{ formatCurrency(scope.row.currentBalance) }}
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="creditLimit" label="Kredi Limiti" width="120" align="right">
          <template #default="scope">
            {{ formatCurrency(scope.row.creditLimit) }}
          </template>
        </el-table-column>
        <el-table-column label="Durum" width="100" align="center">
          <template #default="scope">
            <el-tag :type="scope.row.isActive ? 'success' : 'danger'" size="small">
              {{ scope.row.isActive ? 'Aktif' : 'Pasif' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="İşlemler" width="150" align="center">
          <template #default="scope">
            <el-button size="small" @click="editSupplier(scope.row)">Düzenle</el-button>
            <el-button size="small" type="danger" @click="deleteSupplier(scope.row.supplierId)">Sil</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- Create/Edit Supplier Dialog -->
    <el-dialog
      v-model="showCreateDialog"
      :title="editingSupplier ? 'Tedarikçi Düzenle' : 'Yeni Tedarikçi Ekle'"
      width="600px"
    >
      <el-form :model="supplierForm" :rules="supplierRules" ref="supplierFormRef" label-width="120px">
        <el-form-item label="Tedarikçi Adı" prop="supplierName">
          <el-input v-model="supplierForm.supplierName" />
        </el-form-item>
        
        <el-form-item label="Şirket Adı">
          <el-input v-model="supplierForm.companyName" />
        </el-form-item>
        
        <el-form-item label="İletişim Kişisi">
          <el-input v-model="supplierForm.contactPerson" />
        </el-form-item>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Telefon">
              <el-input v-model="supplierForm.phoneNumber" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="E-posta">
              <el-input v-model="supplierForm.email" type="email" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-form-item label="Adres">
          <el-input type="textarea" v-model="supplierForm.address" />
        </el-form-item>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Vergi No">
              <el-input v-model="supplierForm.taxNumber" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Vergi Dairesi">
              <el-input v-model="supplierForm.taxOffice" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Kredi Limiti">
              <el-input-number v-model="supplierForm.creditLimit" :precision="2" :min="0" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Mevcut Bakiye">
              <el-input-number v-model="supplierForm.currentBalance" :precision="2" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-form-item label="Aktif">
          <el-switch v-model="supplierForm.isActive" />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="showCreateDialog = false">İptal</el-button>
        <el-button type="primary" @click="saveSupplier" :loading="saving">
          {{ editingSupplier ? 'Güncelle' : 'Kaydet' }}
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { supplierService } from '../services/supplierService'
import { ElMessage, ElMessageBox } from 'element-plus'

export default {
  name: 'Suppliers',
  setup() {
    const loading = ref(false)
    const saving = ref(false)
    const suppliers = ref([])
    const searchQuery = ref('')
    const showCreateDialog = ref(false)
    const editingSupplier = ref(null)
    const supplierFormRef = ref()

    const supplierForm = ref({
      supplierName: '',
      companyName: '',
      contactPerson: '',
      phoneNumber: '',
      email: '',
      address: '',
      taxNumber: '',
      taxOffice: '',
      creditLimit: 0,
      currentBalance: 0,
      isActive: true
    })

    const supplierRules = {
      supplierName: [
        { required: true, message: 'Tedarikçi adı gereklidir', trigger: 'blur' }
      ]
    }

    const filteredSuppliers = computed(() => {
      if (!searchQuery.value) return suppliers.value
      
      const query = searchQuery.value.toLowerCase()
      return suppliers.value.filter(s => 
        s.supplierName.toLowerCase().includes(query) ||
        s.companyName?.toLowerCase().includes(query) ||
        s.contactPerson?.toLowerCase().includes(query) ||
        s.email?.toLowerCase().includes(query) ||
        s.phoneNumber?.includes(query)
      )
    })

    const loadSuppliers = async () => {
      loading.value = true
      try {
        suppliers.value = await supplierService.getAllSuppliers()
      } catch (error) {
        ElMessage.error('Tedarikçiler yüklenirken hata oluştu')
      } finally {
        loading.value = false
      }
    }

    const handleSearch = () => {
      // Search is handled by computed property
    }

    const resetFilters = () => {
      searchQuery.value = ''
    }

    const editSupplier = (supplier) => {
      editingSupplier.value = supplier
      supplierForm.value = { ...supplier }
      showCreateDialog.value = true
    }

    const saveSupplier = async () => {
      if (!supplierFormRef.value) return
      
      const valid = await supplierFormRef.value.validate()
      if (!valid) return

      saving.value = true
      try {
        if (editingSupplier.value) {
          await supplierService.updateSupplier(editingSupplier.value.supplierId, supplierForm.value)
          ElMessage.success('Tedarikçi güncellendi')
        } else {
          await supplierService.createSupplier(supplierForm.value)
          ElMessage.success('Tedarikçi eklendi')
        }
        
        showCreateDialog.value = false
        resetSupplierForm()
        await loadSuppliers()
      } catch (error) {
        ElMessage.error('Tedarikçi kaydedilirken hata oluştu')
      } finally {
        saving.value = false
      }
    }

    const deleteSupplier = async (supplierId) => {
      try {
        await ElMessageBox.confirm(
          'Bu tedarikçiyi silmek istediğinizden emin misiniz?',
          'Uyarı',
          {
            confirmButtonText: 'Evet',
            cancelButtonText: 'Hayır',
            type: 'warning',
          }
        )

        await supplierService.deleteSupplier(supplierId)
        ElMessage.success('Tedarikçi silindi')
        await loadSuppliers()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('Tedarikçi silinirken hata oluştu')
        }
      }
    }

    const resetSupplierForm = () => {
      supplierForm.value = {
        supplierName: '',
        companyName: '',
        contactPerson: '',
        phoneNumber: '',
        email: '',
        address: '',
        taxNumber: '',
        taxOffice: '',
        creditLimit: 0,
        currentBalance: 0,
        isActive: true
      }
      editingSupplier.value = null
    }

    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('tr-TR', {
        style: 'currency',
        currency: 'TRY'
      }).format(amount)
    }

    onMounted(() => {
      loadSuppliers()
    })

    return {
      loading,
      saving,
      suppliers,
      searchQuery,
      showCreateDialog,
      editingSupplier,
      supplierForm,
      supplierFormRef,
      supplierRules,
      filteredSuppliers,
      loadSuppliers,
      handleSearch,
      resetFilters,
      editSupplier,
      saveSupplier,
      deleteSupplier,
      formatCurrency
    }
  }
}
</script>

<style scoped>
.suppliers-page {
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

.negative-balance {
  color: #f56c6c;
  font-weight: 600;
}

.positive-balance {
  color: #67c23a;
  font-weight: 600;
}

.el-card {
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}
</style>
