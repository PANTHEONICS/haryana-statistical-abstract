const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api';

class ApiService {
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
          // Token expired or invalid
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
      console.error('API request failed:', error);
      throw error;
    }
  }

  // Auth endpoints
  async register(data) {
    return this.request('/auth/register', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  async login(usernameOrEmail, password) {
    return this.request('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ usernameOrEmail, password }),
    });
  }

  async refreshToken(refreshToken) {
    return this.request('/auth/refresh', {
      method: 'POST',
      body: JSON.stringify({ refreshToken }),
    });
  }

  async logout() {
    return this.request('/auth/logout', {
      method: 'POST',
    });
  }

  async getCurrentUser() {
    return this.request('/auth/me');
  }

  // User management endpoints (now using UserManagementController)
  async getAllUsers() {
    return this.request('/usermanagement/users');
  }

  async getUserById(id) {
    return this.request(`/usermanagement/users/${id}`);
  }

  async updateUser(id, data) {
    return this.request(`/usermanagement/users/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  async deleteUser(id) {
    return this.request(`/usermanagement/users/${id}`, {
      method: 'DELETE',
    });
  }

  async getAllRoles() {
    return this.request('/usermanagement/roles');
  }

  async getAllDepartments() {
    return this.request('/usermanagement/departments');
  }

  async assignRole(userId, roleId) {
    // This functionality is now part of CreateUser/UpdateUser in UserManagement
    // Role is assigned during user creation/update
    return this.request(`/usermanagement/users/${userId}`, {
      method: 'PUT',
      body: JSON.stringify({ roleId }),
    });
  }

  async removeRole(userId, roleId) {
    // Role assignment is managed during user update
    // This endpoint is deprecated - use updateUser instead
    return this.request(`/usermanagement/users/${userId}`, {
      method: 'PUT',
      body: JSON.stringify({ roleId: null }),
    });
  }
}

export default new ApiService();
