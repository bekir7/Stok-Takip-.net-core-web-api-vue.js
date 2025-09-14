<template>
  <div id="app">
    <el-container>
      <el-header class="app-header">
        <div class="header-content">
          <h1 class="app-title">
            <el-icon><Box /></el-icon>
            Stok Takip Sistemi
          </h1>
          <nav class="nav-menu">
            <router-link to="/" class="nav-item">
              <el-icon><House /></el-icon>
              Dashboard
            </router-link>
            <router-link to="/products" class="nav-item">
              <el-icon><Goods /></el-icon>
              Ürünler
            </router-link>
            <router-link to="/customers" class="nav-item">
              <el-icon><User /></el-icon>
              Müşteriler
            </router-link>
            <router-link to="/suppliers" class="nav-item">
              <el-icon><OfficeBuilding /></el-icon>
              Tedarikçiler
            </router-link>
            <router-link to="/sales" class="nav-item">
              <el-icon><ShoppingCart /></el-icon>
              Satışlar
            </router-link>
            <router-link to="/purchases" class="nav-item">
              <el-icon><ShoppingBag /></el-icon>
              Alışlar
            </router-link>
            <router-link to="/stock-movements" class="nav-item">
              <el-icon><TrendCharts /></el-icon>
              Stok Hareketleri
            </router-link>
          </nav>
        </div>
      </el-header>
      
      <el-main class="app-main">
        <router-view />
      </el-main>
    </el-container>
  </div>
</template>

<script>
export default {
  name: 'App'
}
</script>

<style scoped>
.app-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 0;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}

.header-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 100%;
  padding: 0 20px;
}

.app-title {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 8px;
}

.nav-menu {
  display: flex;
  gap: 20px;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 16px;
  color: white;
  text-decoration: none;
  border-radius: 6px;
  transition: all 0.3s ease;
  font-weight: 500;
}

.nav-item:hover {
  background: rgba(255, 255, 255, 0.1);
  transform: translateY(-1px);
}

.nav-item.router-link-active {
  background: rgba(255, 255, 255, 0.2);
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.app-main {
  background: #f5f7fa;
  min-height: calc(100vh - 60px);
  padding: 20px;
}

/* Global styles */
#app {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

* {
  box-sizing: border-box;
}

body {
  margin: 0;
  padding: 0;
}
</style>
