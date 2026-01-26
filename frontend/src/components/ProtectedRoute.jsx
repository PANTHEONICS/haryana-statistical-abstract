import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useState, useEffect } from 'react';
import menuApi from '../services/menuApi';
import { Loader2 } from 'lucide-react';

const ProtectedRoute = ({ children, requireAdmin = false }) => {
  const { isAuthenticated, loading: authLoading, user } = useAuth();
  const location = useLocation();
  const [checkingMenuAccess, setCheckingMenuAccess] = useState(false);
  const [hasMenuAccess, setHasMenuAccess] = useState(true);
  const [checkingComplete, setCheckingComplete] = useState(false);

  useEffect(() => {
    checkMenuAccess();
  }, [location.pathname, user]);

  const checkMenuAccess = async () => {
    if (!isAuthenticated || authLoading) {
      setCheckingComplete(true);
      return;
    }

    // Admin has access to all pages
    const isAdmin = user?.roles?.includes('Admin') || 
                    user?.roles?.includes('System Admin') || 
                    user?.RoleName === 'System Admin';

    if (isAdmin || requireAdmin) {
      setHasMenuAccess(true);
      setCheckingComplete(true);
      return;
    }

    // For non-admin users, check menu access
    try {
      setCheckingMenuAccess(true);
      const result = await menuApi.checkMenuAccess(location.pathname);
      setHasMenuAccess(result.hasAccess);
    } catch (error) {
      console.error('Error checking menu access:', error);
      setHasMenuAccess(false);
    } finally {
      setCheckingMenuAccess(false);
      setCheckingComplete(true);
    }
  };

  if (authLoading || !checkingComplete || checkingMenuAccess) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  // Check admin requirement
  const isAdmin = user?.roles?.includes('Admin') || 
                  user?.roles?.includes('System Admin') || 
                  user?.RoleName === 'System Admin';

  if (requireAdmin && !isAdmin) {
    return <Navigate to="/" replace />;
  }

  // Check menu access for non-admin users
  if (!hasMenuAccess && !isAdmin) {
    return (
      <div className="flex flex-col items-center justify-center min-h-screen p-4">
        <div className="text-center max-w-md">
          <h1 className="text-2xl font-bold text-destructive mb-2">Access Denied</h1>
          <p className="text-muted-foreground mb-4">
            You don't have permission to access this page.
          </p>
          <p className="text-sm text-muted-foreground">
            If you believe this is an error, please contact your administrator.
          </p>
        </div>
      </div>
    );
  }

  return children;
};

export default ProtectedRoute;
