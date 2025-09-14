import api from './api'

export const stockMovementService = {
  // Get all stock movements
  async getAllStockMovements() {
    try {
      const response = await api.get('/stockmovements')
      return response.data
    } catch (error) {
      console.error('Error fetching stock movements:', error)
      throw error
    }
  },

  // Get stock movement by ID
  async getStockMovementById(id) {
    try {
      const response = await api.get(`/stockmovements/${id}`)
      return response.data
    } catch (error) {
      console.error('Error fetching stock movement:', error)
      throw error
    }
  },

  // Create new stock movement
  async createStockMovement(stockMovementData) {
    try {
      const response = await api.post('/stockmovements', stockMovementData)
      return response.data
    } catch (error) {
      console.error('Error creating stock movement:', error)
      throw error
    }
  },

  // Update stock movement
  async updateStockMovement(id, stockMovementData) {
    try {
      const response = await api.put(`/stockmovements/${id}`, stockMovementData)
      return response.data
    } catch (error) {
      console.error('Error updating stock movement:', error)
      throw error
    }
  },

  // Delete stock movement
  async deleteStockMovement(id) {
    try {
      await api.delete(`/stockmovements/${id}`)
      return true
    } catch (error) {
      console.error('Error deleting stock movement:', error)
      throw error
    }
  },

  // Get stock movements by product
  async getStockMovementsByProduct(productId) {
    try {
      const response = await api.get(`/stockmovements/product/${productId}`)
      return response.data
    } catch (error) {
      console.error('Error fetching stock movements by product:', error)
      throw error
    }
  },

  // Get stock movements by date range
  async getStockMovementsByDateRange(startDate, endDate) {
    try {
      const response = await api.get(`/stockmovements/date-range?startDate=${startDate}&endDate=${endDate}`)
      return response.data
    } catch (error) {
      console.error('Error fetching stock movements by date range:', error)
      throw error
    }
  },

  // Get stock movements by type
  async getStockMovementsByType(type) {
    try {
      const response = await api.get(`/stockmovements/type/${type}`)
      return response.data
    } catch (error) {
      console.error('Error fetching stock movements by type:', error)
      throw error
    }
  }
}
