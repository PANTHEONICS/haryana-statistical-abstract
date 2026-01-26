const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api';

class UserManagementApiService {
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
      console.error('API request failed:', error);
      throw error;
    }
  }

  // Authentication endpoints
  async login(loginID, password) {
    return this.request('/UserManagement/login', {
      method: 'POST',
      body: JSON.stringify({ loginID, password }),
    });
  }

  // User management endpoints
  async createUser(userData) {
    return this.request('/UserManagement/users', {
      method: 'POST',
      body: JSON.stringify(userData),
    });
  }

  async getAllUsers() {
    return this.request('/UserManagement/users');
  }

  async getUserById(id) {
    return this.request(`/UserManagement/users/${id}`);
  }

  async getCurrentUser() {
    return this.request('/UserManagement/me');
  }

  // Master data endpoints
  async getAllRoles() {
    return this.request('/UserManagement/roles');
  }

  async getAllDepartments() {
    return this.request('/UserManagement/departments');
  }

  async getAllSecurityQuestions() {
    return this.request('/UserManagement/security-questions');
  }

  async changePassword(currentPassword, newPassword, confirmPassword) {
    return this.request('/UserManagement/change-password', {
      method: 'POST',
      body: JSON.stringify({
        currentPassword,
        newPassword,
        confirmPassword
      }),
    });
  }
}

export default new UserManagementApiService();
