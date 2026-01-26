const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api';

class WorkflowApiService {
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
      console.error('Workflow API request failed:', error);
      throw error;
    }
  }

  /**
   * Execute a workflow action
   * @param {string} tableName - Target table name (e.g., 'Tbl_PopulationData')
   * @param {number} recordId - Primary key of the record
   * @param {string} action - Action name (SubmitToChecker, CheckerReject, CheckerApprove, HeadReject, HeadApprove)
   * @param {string} remarks - Optional remarks (mandatory for rejections)
   * @returns {Promise} Workflow action response
   */
  async executeAction(tableName, recordId, action, remarks = null) {
    return this.request('/workflow/execute', {
      method: 'POST',
      body: JSON.stringify({
        tableName,
        recordId,
        action,
        remarks,
      }),
    });
  }

  /**
   * Get audit history for a record
   * @param {string} tableName - Target table name
   * @param {number} recordId - Primary key of the record
   * @returns {Promise} List of audit history entries
   */
  async getAuditHistory(tableName, recordId) {
    return this.request(`/workflow/history/${encodeURIComponent(tableName)}/${recordId}`);
  }

  /**
   * Get current workflow status for a record
   * @param {string} tableName - Target table name
   * @param {number} recordId - Primary key of the record
   * @returns {Promise} Current status ID
   */
  async getCurrentStatus(tableName, recordId) {
    const response = await this.request(`/workflow/status/${encodeURIComponent(tableName)}/${recordId}`);
    return response.statusId;
  }

  /**
   * Get all workflow statuses
   * @returns {Promise} List of all workflow statuses
   */
  async getAllStatuses() {
    return this.request('/workflow/statuses');
  }

  // ============================================
  // Screen-Level Workflow Methods
  // ============================================

  /**
   * Execute a screen-level workflow action
   * @param {string} screenCode - Screen code (e.g., 'CENSUS_POPULATION')
   * @param {string} action - Action name (SubmitToChecker, CheckerReject, CheckerApprove, HeadReject, HeadApprove)
   * @param {string} remarks - Optional remarks (mandatory for rejections)
   * @returns {Promise} Workflow action response
   */
  async executeScreenAction(screenCode, action, remarks = null) {
    return this.request('/workflow/screen/execute', {
      method: 'POST',
      body: JSON.stringify({
        screenCode,
        action,
        remarks,
      }),
    });
  }

  /**
   * Get audit history for a screen
   * @param {string} screenCode - Screen code
   * @returns {Promise} List of audit history entries
   */
  async getScreenAuditHistory(screenCode) {
    return this.request(`/workflow/screen/history/${encodeURIComponent(screenCode)}`);
  }

  /**
   * Get current workflow status for a screen
   * @param {string} screenCode - Screen code
   * @returns {Promise} Current status ID
   */
  async getScreenCurrentStatus(screenCode) {
    const response = await this.request(`/workflow/screen/status/${encodeURIComponent(screenCode)}`);
    return response.statusId;
  }

  /**
   * Reset screen workflow to draft phase (Admin only - for testing)
   * Clears all audit history and resets status to Draft
   * @param {string} screenCode - Screen code
   * @returns {Promise} Workflow action response
   */
  async resetScreenWorkflow(screenCode) {
    return this.request(`/workflow/screen/reset/${encodeURIComponent(screenCode)}`, {
      method: 'POST',
    });
  }
}

export default new WorkflowApiService();
