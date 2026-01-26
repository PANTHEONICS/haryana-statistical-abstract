import { useState, useCallback } from 'react';

/**
 * Reusable Form Dialog Management Hook
 * Handles create/edit dialog state and form data management
 * 
 * @param {object} initialFormData - Initial form data structure
 * @param {function} formMapper - Optional function to map record to form data format
 * @returns {object} - { dialogOpen, editingRecord, formData, setFormData, openCreate, openEdit, closeDialog, isEditMode, resetForm }
 */
export function useFormDialog(initialFormData, formMapper = null) {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingRecord, setEditingRecord] = useState(null);
  const [formData, setFormData] = useState(initialFormData);

  /**
   * Map record to form data format
   */
  const mapToFormData = useCallback((record) => {
    if (!record) return initialFormData;
    
    if (formMapper) {
      return formMapper(record);
    }
    
    // Default: use record as-is, but ensure all initialFormData keys exist
    return { ...initialFormData, ...record };
  }, [initialFormData, formMapper]);

  /**
   * Open dialog in create mode
   */
  const openCreate = useCallback(() => {
    setEditingRecord(null);
    setFormData(initialFormData);
    setDialogOpen(true);
  }, [initialFormData]);

  /**
   * Open dialog in edit mode
   * @param {object} record - Record to edit
   */
  const openEdit = useCallback((record) => {
    if (!record) {
      console.warn('useFormDialog: openEdit called without a record');
      return;
    }
    
    setEditingRecord(record);
    const mappedData = mapToFormData(record);
    setFormData(mappedData);
    setDialogOpen(true);
  }, [mapToFormData]);

  /**
   * Close dialog and reset state
   */
  const closeDialog = useCallback(() => {
    setDialogOpen(false);
    setEditingRecord(null);
    setFormData(initialFormData);
  }, [initialFormData]);

  /**
   * Reset form to initial state
   */
  const resetForm = useCallback(() => {
    setFormData(initialFormData);
    setEditingRecord(null);
  }, [initialFormData]);

  /**
   * Check if dialog is in edit mode
   */
  const isEditMode = editingRecord !== null;

  /**
   * Update a single form field
   * @param {string} fieldName - Field name
   * @param {any} value - Field value
   */
  const updateField = useCallback((fieldName, value) => {
    setFormData(prev => ({
      ...prev,
      [fieldName]: value
    }));
  }, []);

  /**
   * Update multiple form fields at once
   * @param {object} updates - Object with field names as keys
   */
  const updateFields = useCallback((updates) => {
    setFormData(prev => ({
      ...prev,
      ...updates
    }));
  }, []);

  return {
    // State
    dialogOpen,
    editingRecord,
    formData,
    isEditMode,
    
    // Setters
    setFormData,
    setDialogOpen,
    setEditingRecord,
    
    // Actions
    openCreate,
    openEdit,
    closeDialog,
    resetForm,
    
    // Field updates
    updateField,
    updateFields,
  };
}

export default useFormDialog;
