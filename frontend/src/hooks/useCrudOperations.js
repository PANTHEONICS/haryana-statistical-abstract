import { useState, useEffect, useCallback } from 'react';

/**
 * Reusable CRUD Operations Hook
 * Handles loading, creating, updating, and deleting records
 * 
 * @param {object} apiService - API service instance (must have getAll, create, update, delete methods)
 * @param {function} dataMapper - Optional function to map API response to frontend format
 * @param {object} options - Configuration options
 * @returns {object} - { data, loading, error, loadData, createRecord, updateRecord, deleteRecord, refreshData }
 */
export function useCrudOperations(apiService, dataMapper = null, options = {}) {
  const {
    autoLoad = true, // Automatically load data on mount
    onSuccess = null, // Callback after successful operations
    onError = null, // Callback after errors
    confirmDelete = true, // Show confirmation dialog before delete
    deleteMessage = (record) => `Are you sure you want to delete this record?`,
  } = options;

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(autoLoad);
  const [error, setError] = useState(null);

  /**
   * Map data using provided mapper function
   */
  const mapData = useCallback((records) => {
    if (!dataMapper || !records) return records;
    
    if (Array.isArray(records)) {
      return records.map(dataMapper);
    }
    return dataMapper(records);
  }, [dataMapper]);

  /**
   * Load all records from API
   */
  const loadData = useCallback(async () => {
    if (!apiService) {
      console.warn('useCrudOperations: apiService is not provided');
      return;
    }

    try {
      setLoading(true);
      setError(null);
      
      const records = await apiService.getAll();
      const mappedRecords = mapData(records);
      setData(mappedRecords || []);
      
      if (onSuccess) {
        onSuccess('load', mappedRecords);
      }
    } catch (err) {
      const errorMessage = err.message || 'Failed to load data';
      setError(errorMessage);
      console.error('Failed to load data:', err);
      
      if (onError) {
        onError('load', err);
      } else {
        // Default error handling - show alert
        alert(errorMessage);
      }
    } finally {
      setLoading(false);
    }
  }, [apiService, mapData, onSuccess, onError]);

  /**
   * Create a new record
   * @param {object} recordData - Data for the new record
   * @returns {Promise<object>} Created record
   */
  const createRecord = useCallback(async (recordData) => {
    if (!apiService) {
      throw new Error('apiService is not provided');
    }

    try {
      setError(null);
      const created = await apiService.create(recordData);
      const mappedRecord = mapData(created);
      
      // Refresh data after creation
      await loadData();
      
      if (onSuccess) {
        onSuccess('create', mappedRecord);
      }
      
      return mappedRecord;
    } catch (err) {
      const errorMessage = err.message || 'Failed to create record';
      setError(errorMessage);
      console.error('Failed to create record:', err);
      
      if (onError) {
        onError('create', err);
      } else {
        alert(errorMessage);
      }
      throw err;
    }
  }, [apiService, mapData, loadData, onSuccess, onError]);

  /**
   * Update an existing record
   * @param {number|string} id - Record ID
   * @param {object} recordData - Updated data
   * @returns {Promise<object>} Updated record
   */
  const updateRecord = useCallback(async (id, recordData) => {
    if (!apiService) {
      throw new Error('apiService is not provided');
    }

    try {
      setError(null);
      const updated = await apiService.update(id, recordData);
      const mappedRecord = mapData(updated);
      
      // Refresh data after update
      await loadData();
      
      if (onSuccess) {
        onSuccess('update', mappedRecord);
      }
      
      return mappedRecord;
    } catch (err) {
      const errorMessage = err.message || 'Failed to update record';
      setError(errorMessage);
      console.error('Failed to update record:', err);
      
      if (onError) {
        onError('update', err);
      } else {
        alert(errorMessage);
      }
      throw err;
    }
  }, [apiService, mapData, loadData, onSuccess, onError]);

  /**
   * Delete a record
   * @param {number|string} id - Record ID
   * @param {object} record - Optional record object for confirmation message
   * @returns {Promise<void>}
   */
  const deleteRecord = useCallback(async (id, record = null) => {
    if (!apiService) {
      throw new Error('apiService is not provided');
    }

    // Show confirmation dialog if enabled
    if (confirmDelete) {
      const message = typeof deleteMessage === 'function' 
        ? deleteMessage(record || { id }) 
        : deleteMessage;
      
      if (!window.confirm(message)) {
        return; // User cancelled
      }
    }

    try {
      setError(null);
      await apiService.delete(id);
      
      // Refresh data after deletion
      await loadData();
      
      if (onSuccess) {
        onSuccess('delete', { id });
      }
    } catch (err) {
      const errorMessage = err.message || 'Failed to delete record';
      setError(errorMessage);
      console.error('Failed to delete record:', err);
      
      if (onError) {
        onError('delete', err);
      } else {
        alert(errorMessage);
      }
      throw err;
    }
  }, [apiService, loadData, confirmDelete, deleteMessage, onSuccess, onError]);

  /**
   * Refresh data (alias for loadData)
   */
  const refreshData = useCallback(() => {
    return loadData();
  }, [loadData]);

  // Auto-load data on mount if enabled
  useEffect(() => {
    if (autoLoad && apiService) {
      loadData();
    }
  }, [autoLoad, apiService]); // Only run on mount or when apiService changes

  return {
    // State
    data,
    loading,
    error,
    
    // Actions
    loadData,
    createRecord,
    updateRecord,
    deleteRecord,
    refreshData,
    
    // Setters (for manual control)
    setData,
    setLoading,
    setError,
  };
}

export default useCrudOperations;
