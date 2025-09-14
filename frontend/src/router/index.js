import { createRouter, createWebHistory } from 'vue-router'
import Dashboard from '../views/Dashboard.vue'
import Products from '../views/Products.vue'
import Customers from '../views/Customers.vue'
import Suppliers from '../views/Suppliers.vue'
import Sales from '../views/Sales.vue'
import Purchases from '../views/Purchases.vue'
import StockMovements from '../views/StockMovements.vue'

const routes = [
  {
    path: '/',
    name: 'Dashboard',
    component: Dashboard
  },
  {
    path: '/products',
    name: 'Products',
    component: Products
  },
  {
    path: '/customers',
    name: 'Customers',
    component: Customers
  },
  {
    path: '/suppliers',
    name: 'Suppliers',
    component: Suppliers
  },
  {
    path: '/sales',
    name: 'Sales',
    component: Sales
  },
  {
    path: '/purchases',
    name: 'Purchases',
    component: Purchases
  },
  {
    path: '/stock-movements',
    name: 'StockMovements',
    component: StockMovements
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
