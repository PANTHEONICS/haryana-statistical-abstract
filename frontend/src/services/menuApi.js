const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api';

class MenuApiService {
  constructor() {
    this.baseURL = API_BASE_URL;
  }

  async request(endpoint, options = {}) {
    const url = `${this.baseURL}${endpoint}`;
    const token = localStorage.getItem('accessToken');

    const config = {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...(token && { Authorization: `Bearer ${token}` }),
        ...options.headers,
      },
    };

    try {
      const response = await fetch(url, config);
      
      if (!response.ok) {
        if (response.status === 401) {
          localStorage.removeItem('accessToken');
          localStorage.removeItem('refreshToken');
          localStorage.removeItem('user');
          window.location.href = '/login';
          throw new Error('Unauthorized');
        }
        
        const error = await response.json().catch(() => ({ message: 'An error occurred' }));
        throw new Error(error.message || 'Request failed');
      }

      return await response.json();
    } catch (error) {
      console.error('Menu API request failed:', error);
      throw error;
    }
  }

  // Get all menus (Admin only)
  async getAllMenus() {
    return this.request('/menu');
  }

  // Get user's accessible menus
  async getUserMenus() {
    return this.request('/menu/user-menus');
  }

  // Check menu access
  async checkMenuAccess(menuPath) {
    return this.request(`/menu/check-access?menuPath=${encodeURIComponent(menuPath)}`);
  }

  // Get department menu mappings (Admin only)
  async getDepartmentMenuMappings() {
    return this.request('/menu/department-mappings');
  }

  // Get menus assigned to a department
  async getDepartmentMenus(departmentId) {
    return this.request(`/menu/department/${departmentId}`);
  }

  // Assign menus to department
  async assignMenusToDepartment(departmentId, menuIds) {
    return this.request('/menu/assign-to-department', {
      method: 'POST',
      body: JSON.stringify({
        departmentID: departmentId,
        menuIDs: menuIds,
      }),
    });
  }
}

export default new MenuApiService();
