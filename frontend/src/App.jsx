import { BrowserRouter, useLocation } from 'react-router-dom'
import AppLayout from './components/layout/AppLayout'
import { Router } from './router'
import { AuthProvider } from './contexts/AuthContext'

function AppContent() {
  const location = useLocation()
  const isAuthPage = location.pathname === '/login' || location.pathname === '/register'

  if (isAuthPage) {
    return <Router />
  }

  return (
    <AppLayout>
      <Router />
    </AppLayout>
  )
}

function App() {
  return (
    <AuthProvider>
      <BrowserRouter
        future={{
          v7_startTransition: true,
          v7_relativeSplatPath: true,
        }}
      >
        <AppContent />
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App
