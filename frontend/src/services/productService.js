import api from './api'

export const productService = {
  // Get all products
  async getAllProducts() {
    try {
      console.log('Fetching products...')
      const response = await api.get('/products')
      console.log('Products fetched successfully:', response.data)
      return response.data
    } catch (error) {
      console.error('Error fetching products:', error)
      console.error('Error response:', error.response?.data)
      throw error
    }
  },

  // Get product by ID
  async getProductById(id) {
    try {
      const response = await api.get(`/products/${id}`)
      return response.data
    } catch (error) {
      console.error('Error fetching product:', error)
      throw error
    }
  },

  // Create new product
  async createProduct(productData) {
    try {
      console.log('Sending product data:', productData)
      const response = await api.post('/products', productData)
      return response.data
    } catch (error) {
      console.error('Error creating product:', error)
      console.error('Error response:', error.response?.data)
      throw error
    }
  },

  // Update product
  async updateProduct(id, productData) {
    try {
      const response = await api.put(`/products/${id}`, productData)
      return response.data
    } catch (error) {
      console.error('Error updating product:', error)
      throw error
    }
  },

  // Delete product
  async deleteProduct(id) {
    try {
      await api.delete(`/products/${id}`)
      return true
    } catch (error) {
      console.error('Error deleting product:', error)
      throw error
    }
  },

  // Get low stock products
  async getLowStockProducts() {
    try {
      const response = await api.get('/products/low-stock')
      return response.data
    } catch (error) {
      console.error('Error fetching low stock products:', error)
      throw error
    }
  },

  // Search products
  async searchProducts(query) {
    try {
      const response = await api.get(`/products/search?q=${encodeURIComponent(query)}`)
      return response.data
    } catch (error) {
      console.error('Error searching products:', error)
      throw error
    }
  },

  // Update stock quantity
  async updateStock(id, quantity, description) {
    try {
      await api.post(`/products/${id}/stock`, {
        quantity,
        description
      })
      return true
    } catch (error) {
      console.error('Error updating stock:', error)
      throw error
    }
  }
}
