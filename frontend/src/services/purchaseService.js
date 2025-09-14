import api from './api'

export const purchaseService = {
  // Get all purchases
  async getAllPurchases() {
    try {
      const response = await api.get('/purchases')
      return response.data
    } catch (error) {
      console.error('Error fetching purchases:', error)
      throw error
    }
  },

  // Get purchase by ID
  async getPurchaseById(id) {
    try {
      const response = await api.get(`/purchases/${id}`)
      return response.data
    } catch (error) {
      console.error('Error fetching purchase:', error)
      throw error
    }
  },

  // Create new purchase
  async createPurchase(purchaseData) {
    try {
      const response = await api.post('/purchases', purchaseData)
      return response.data
    } catch (error) {
      console.error('Error creating purchase:', error)
      throw error
    }
  },

  // Update purchase
  async updatePurchase(id, purchaseData) {
    try {
      const response = await api.put(`/purchases/${id}`, purchaseData)
      return response.data
    } catch (error) {
      console.error('Error updating purchase:', error)
      throw error
    }
  },

  // Delete purchase
  async deletePurchase(id) {
    try {
      await api.delete(`/purchases/${id}`)
      return true
    } catch (error) {
      console.error('Error deleting purchase:', error)
      throw error
    }
  },

  // Get purchases by date range
  async getPurchasesByDateRange(startDate, endDate) {
    try {
      const response = await api.get(`/purchases/date-range?startDate=${startDate}&endDate=${endDate}`)
      return response.data
    } catch (error) {
      console.error('Error fetching purchases by date range:', error)
      throw error
    }
  },

  // Get purchases by supplier
  async getPurchasesBySupplier(supplierId) {
    try {
      const response = await api.get(`/purchases/supplier/${supplierId}`)
      return response.data
    } catch (error) {
      console.error('Error fetching purchases by supplier:', error)
      throw error
    }
  }
}
