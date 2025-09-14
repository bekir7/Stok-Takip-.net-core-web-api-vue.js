<template>
  <div class="customers-page">
    <div class="page-header">
      <h2>Müşteri Yönetimi</h2>
      <el-button type="primary" @click="showCreateDialog = true">
        <el-icon><Plus /></el-icon>
        Yeni Müşteri Ekle
      </el-button>
    </div>

    <!-- Search -->
    <el-card class="filter-card">
      <el-row :gutter="20">
        <el-col :span="8">
          <el-input
            v-model="searchQuery"
            placeholder="Müşteri ara..."
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

    <!-- Customers Table -->
    <el-card>
      <el-table :data="filteredCustomers" v-loading="loading" style="width: 100%">
        <el-table-column prop="customerName" label="Müşteri Adı" />
        <el-table-column prop="companyName" label="Şirket" />
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
            <el-button size="small" @click="editCustomer(scope.row)">Düzenle</el-button>
            <el-button size="small" type="danger" @click="deleteCustomer(scope.row.customerId)">Sil</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- Create/Edit Customer Dialog -->
    <el-dialog
      v-model="showCreateDialog"
      :title="editingCustomer ? 'Müşteri Düzenle' : 'Yeni Müşteri Ekle'"
      width="600px"
    >
      <el-form :model="customerForm" :rules="customerRules" ref="customerFormRef" label-width="120px">
        <el-form-item label="Müşteri Adı" prop="customerName">
          <el-input v-model="customerForm.customerName" />
        </el-form-item>
        
        <el-form-item label="Şirket Adı">
          <el-input v-model="customerForm.companyName" />
        </el-form-item>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Telefon">
              <el-input v-model="customerForm.phoneNumber" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="E-posta">
              <el-input v-model="customerForm.email" type="email" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-form-item label="Adres">
          <el-input type="textarea" v-model="customerForm.address" />
        </el-form-item>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Vergi No">
              <el-input v-model="customerForm.taxNumber" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Vergi Dairesi">
              <el-input v-model="customerForm.taxOffice" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="Kredi Limiti">
              <el-input-number v-model="customerForm.creditLimit" :precision="2" :min="0" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="Mevcut Bakiye">
              <el-input-number v-model="customerForm.currentBalance" :precision="2" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        
        <el-form-item label="Aktif">
          <el-switch v-model="customerForm.isActive" />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="showCreateDialog = false">İptal</el-button>
        <el-button type="primary" @click="saveCustomer" :loading="saving">
          {{ editingCustomer ? 'Güncelle' : 'Kaydet' }}
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue'
import { customerService } from '../services/customerService'
import { ElMessage, ElMessageBox } from 'element-plus'

export default {
  name: 'Customers',
  setup() {
    const loading = ref(false)
    const saving = ref(false)
    const customers = ref([])
    const searchQuery = ref('')
    const showCreateDialog = ref(false)
    const editingCustomer = ref(null)
    const customerFormRef = ref()

    const customerForm = ref({
      customerName: '',
      companyName: '',
      phoneNumber: '',
      email: '',
      address: '',
      taxNumber: '',
      taxOffice: '',
      creditLimit: 0,
      currentBalance: 0,
      isActive: true
    })

    const customerRules = {
      customerName: [
        { required: true, message: 'Müşteri adı gereklidir', trigger: 'blur' }
      ]
    }

    const filteredCustomers = computed(() => {
      if (!searchQuery.value) return customers.value
      
      const query = searchQuery.value.toLowerCase()
      return customers.value.filter(c => 
        c.customerName.toLowerCase().includes(query) ||
        c.companyName?.toLowerCase().includes(query) ||
        c.email?.toLowerCase().includes(query) ||
        c.phoneNumber?.includes(query)
      )
    })

    const loadCustomers = async () => {
      loading.value = true
      try {
        customers.value = await customerService.getAllCustomers()
      } catch (error) {
        ElMessage.error('Müşteriler yüklenirken hata oluştu')
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

    const editCustomer = (customer) => {
      editingCustomer.value = customer
      customerForm.value = { ...customer }
      showCreateDialog.value = true
    }

    const saveCustomer = async () => {
      if (!customerFormRef.value) return
      
      const valid = await customerFormRef.value.validate()
      if (!valid) return

      saving.value = true
      try {
        if (editingCustomer.value) {
          await customerService.updateCustomer(editingCustomer.value.customerId, customerForm.value)
          ElMessage.success('Müşteri güncellendi')
        } else {
          await customerService.createCustomer(customerForm.value)
          ElMessage.success('Müşteri eklendi')
        }
        
        showCreateDialog.value = false
        resetCustomerForm()
        await loadCustomers()
      } catch (error) {
        ElMessage.error('Müşteri kaydedilirken hata oluştu')
      } finally {
        saving.value = false
      }
    }

    const deleteCustomer = async (customerId) => {
      try {
        await ElMessageBox.confirm(
          'Bu müşteriyi silmek istediğinizden emin misiniz?',
          'Uyarı',
          {
            confirmButtonText: 'Evet',
            cancelButtonText: 'Hayır',
            type: 'warning',
          }
        )

        await customerService.deleteCustomer(customerId)
        ElMessage.success('Müşteri silindi')
        await loadCustomers()
      } catch (error) {
        if (error !== 'cancel') {
          ElMessage.error('Müşteri silinirken hata oluştu')
        }
      }
    }

    const resetCustomerForm = () => {
      customerForm.value = {
        customerName: '',
        companyName: '',
        phoneNumber: '',
        email: '',
        address: '',
        taxNumber: '',
        taxOffice: '',
        creditLimit: 0,
        currentBalance: 0,
        isActive: true
      }
      editingCustomer.value = null
    }

    const formatCurrency = (amount) => {
      return new Intl.NumberFormat('tr-TR', {
        style: 'currency',
        currency: 'TRY'
      }).format(amount)
    }

    onMounted(() => {
      loadCustomers()
    })

    return {
      loading,
      saving,
      customers,
      searchQuery,
      showCreateDialog,
      editingCustomer,
      customerForm,
      customerFormRef,
      customerRules,
      filteredCustomers,
      loadCustomers,
      handleSearch,
      resetFilters,
      editCustomer,
      saveCustomer,
      deleteCustomer,
      formatCurrency
    }
  }
}
</script>

<style scoped>
.customers-page {
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
