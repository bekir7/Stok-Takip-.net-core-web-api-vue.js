import api from './api'

export const saleService = {
  // Get all sales
  async getAllSales() {
    try {
      const response = await api.get('/sales')
      return response.data
    } catch (error) {
      console.error('Error fetching sales:', error)
      throw error
    }
  },

  // Get sale by ID
  async getSaleById(id) {
    try {
      const response = await api.get(`/sales/${id}`)
      return response.data
    } catch (error) {
      console.error('Error fetching sale:', error)
      throw error
    }
  },

  // Create new sale
  async createSale(saleData) {
    try {
      const response = await api.post('/sales', saleData)
      return response.data
    } catch (error) {
      console.error('Error creating sale:', error)
      throw error
    }
  },

  // Update sale
  async updateSale(id, saleData) {
    try {
      const response = await api.put(`/sales/${id}`, saleData)
      return response.data
    } catch (error) {
      console.error('Error updating sale:', error)
      throw error
    }
  },

  // Delete sale
  async deleteSale(id) {
    try {
      await api.delete(`/sales/${id}`)
      return true
    } catch (error) {
      console.error('Error deleting sale:', error)
      throw error
    }
  },

  // Get sales by date range
  async getSalesByDateRange(startDate, endDate) {
    try {
      const response = await api.get(`/sales/date-range?startDate=${startDate}&endDate=${endDate}`)
      return response.data
    } catch (error) {
      console.error('Error fetching sales by date range:', error)
      throw error
    }
  },

  // Get sales by customer
  async getSalesByCustomer(customerId) {
    try {
      const response = await api.get(`/sales/customer/${customerId}`)
      return response.data
    } catch (error) {
      console.error('Error fetching sales by customer:', error)
      throw error
    }
  }
}
