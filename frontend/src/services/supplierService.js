import api from './api'

export const supplierService = {
  // Get all suppliers
  async getAllSuppliers() {
    try {
      const response = await api.get('/suppliers')
      return response.data
    } catch (error) {
      console.error('Error fetching suppliers:', error)
      throw error
    }
  },

  // Get supplier by ID
  async getSupplierById(id) {
    try {
      const response = await api.get(`/suppliers/${id}`)
      return response.data
    } catch (error) {
      console.error('Error fetching supplier:', error)
      throw error
    }
  },

  // Create new supplier
  async createSupplier(supplierData) {
    try {
      const response = await api.post('/suppliers', supplierData)
      return response.data
    } catch (error) {
      console.error('Error creating supplier:', error)
      throw error
    }
  },

  // Update supplier
  async updateSupplier(id, supplierData) {
    try {
      const response = await api.put(`/suppliers/${id}`, supplierData)
      return response.data
    } catch (error) {
      console.error('Error updating supplier:', error)
      throw error
    }
  },

  // Delete supplier
  async deleteSupplier(id) {
    try {
      await api.delete(`/suppliers/${id}`)
      return true
    } catch (error) {
      console.error('Error deleting supplier:', error)
      throw error
    }
  },

  // Search suppliers
  async searchSuppliers(query) {
    try {
      const response = await api.get(`/suppliers/search?q=${encodeURIComponent(query)}`)
      return response.data
    } catch (error) {
      console.error('Error searching suppliers:', error)
      throw error
    }
  }
}
