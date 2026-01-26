/**
 * Base API Service Class
 * Provides standardized CRUD operations for all API services
 * 
 * Usage:
 *   const myApi = new BaseApiService('/api/v1/MyEntity')
 *   await myApi.getAll()
 *   await myApi.getById(1)
 *   await myApi.create(data)
 *   await myApi.update(1, data)
 *   await myApi.delete(1)
 */
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api';

export class BaseApiService {
  constructor(endpoint) {
    this.baseURL = API_BASE_URL;
    this.endpoint = endpoint;
  }

  /**
   * Make an API request with authentication and error handling
   * @param {string} path - API path (relative to endpoint)
   * @param {object} options - Fetch options
   * @returns {Promise} Response data
   */
  async request(path = '', options = {}) {
    const url = `${this.baseURL}${this.endpoint}${path}`;
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
          // Unauthorized - clear tokens and redirect to login
          localStorage.removeItem('accessToken');
          localStorage.removeItem('refreshToken');
          localStorage.removeItem('user');
          window.location.href = '/login';
          throw new Error('Unauthorized');
        }
        
        // Try to parse error message from response
        const error = await response.json().catch(() => ({ message: 'An error occurred' }));
        throw new Error(error.message || `Request failed with status ${response.status}`);
      }

      // Handle 204 No Content responses
      if (response.status === 204) {
        return null;
      }

      return await response.json();
    } catch (error) {
      console.error(`API request failed [${this.endpoint}${path}]:`, error);
      throw error;
    }
  }

  /**
   * Get all records
   * @param {object} params - Query parameters (optional)
   * @returns {Promise<Array>} List of records
   */
  async getAll(params = {}) {
    const queryString = new URLSearchParams(params).toString();
    const path = queryString ? `?${queryString}` : '';
    return this.request(path);
  }

  /**
   * Get a single record by ID
   * @param {number|string} id - Record ID
   * @returns {Promise<object>} Record data
   */
  async getById(id) {
    return this.request(`/${id}`);
  }

  /**
   * Create a new record
   * @param {object} data - Record data
   * @returns {Promise<object>} Created record
   */
  async create(data) {
    return this.request('', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  }

  /**
   * Update an existing record
   * @param {number|string} id - Record ID
   * @param {object} data - Updated record data
   * @returns {Promise<object>} Updated record
   */
  async update(id, data) {
    return this.request(`/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    });
  }

  /**
   * Delete a record
   * @param {number|string} id - Record ID
   * @returns {Promise<void>}
   */
  async delete(id) {
    return this.request(`/${id}`, {
      method: 'DELETE',
    });
  }

  /**
   * Custom request method for special endpoints
   * @param {string} path - Custom path
   * @param {object} options - Fetch options
   * @returns {Promise} Response data
   */
  async customRequest(path, options = {}) {
    return this.request(path, options);
  }
}

export default BaseApiService;
