import { useState, useEffect } from "react"
import PageHeader from "@/components/layout/PageHeader"
import { DataTable } from "@/components/ui/DataTable"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Download, Plus, Building2, History } from "lucide-react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import WorkflowStatusBar from "@/components/workflow/WorkflowStatusBar"
import ScreenAuditHistoryModal from "@/components/workflow/ScreenAuditHistoryModal"
import table6_1_institutionsApi from "@/services/Education/table6_1_institutionsApi"
import { useAuth } from "@/contexts/AuthContext"
import { useWorkflowLock } from "@/hooks/useWorkflowLock"
import { useCrudOperations } from "@/hooks/useCrudOperations"
import { useFormDialog } from "@/hooks/useFormDialog"
import { exportToCSV } from "@/utils/export"
import { formatNumber } from "@/utils/format"

// Data mapper: Transform API response to frontend format
const mapInstitutionRecord = (record) => ({
  id: record.institutionID || record.institutionType,
  institutionID: record.institutionID,
  institutionType: record.institutionType,
  year196667: record.year196667,
  year197071: record.year197071,
  year198081: record.year198081,
  year199091: record.year199091,
  year200001: record.year200001,
  year201011: record.year201011,
  year201617: record.year201617,
  year201718: record.year201718,
  year201819: record.year201819,
  year201920: record.year201920,
  year202021: record.year202021,
  year202122: record.year202122,
  year202223: record.year202223,
  year202324: record.year202324,
  year202425: record.year202425,
})

// Form mapper: Transform record to form data format
const mapToFormData = (record) => ({
  institutionType: record.institutionType || '',
  year196667: record.year196667 || record.year_1966_67 || '',
  year197071: record.year197071 || record.year_1970_71 || '',
  year198081: record.year198081 || record.year_1980_81 || '',
  year199091: record.year199091 || record.year_1990_91 || '',
  year200001: record.year200001 || record.year_2000_01 || '',
  year201011: record.year201011 || record.year_2010_11 || '',
  year201617: record.year201617 || record.year_2016_17 || '',
  year201718: record.year201718 || record.year_2017_18 || '',
  year201819: record.year201819 || record.year_2018_19 || '',
  year201920: record.year201920 || record.year_2019_20 || '',
  year202021: record.year202021 || record.year_2020_21 || '',
  year202122: record.year202122 || record.year_2021_22 || '',
  year202223: record.year202223 || record.year_2022_23 || '',
  year202324: record.year202324 || record.year_2023_24 || '',
  year202425: record.year202425 || record.year_2024_25 || '',
})

// Year columns for display
const yearColumns = [
  { key: 'year196667', label: '1966-67' },
  { key: 'year197071', label: '1970-71' },
  { key: 'year198081', label: '1980-81' },
  { key: 'year199091', label: '1990-91' },
  { key: 'year200001', label: '2000-01' },
  { key: 'year201011', label: '2010-11' },
  { key: 'year201617', label: '2016-17' },
  { key: 'year201718', label: '2017-18' },
  { key: 'year201819', label: '2018-19' },
  { key: 'year201920', label: '2019-20' },
  { key: 'year202021', label: '2020-21' },
  { key: 'year202122', label: '2021-22' },
  { key: 'year202223', label: '2022-23' },
  { key: 'year202324', label: '2023-24 (P)' },
  { key: 'year202425', label: '2024-25' },
]

export default function Table6_1_Institutions() {
  const { user } = useAuth()
  
  // ✅ Use generalized hooks
  const { isLocked, statusId: screenWorkflowStatus, lockedMessage, checkAndPreventAction, refreshStatus } = useWorkflowLock('ED_TABLE_6_1_INSTITUTIONS')
  
  const { data, loading, createRecord, updateRecord, deleteRecord, refreshData, loadData } = useCrudOperations(
    table6_1_institutionsApi,
    mapInstitutionRecord,
    {
      autoLoad: true,
      deleteMessage: (record) => `Are you sure you want to delete the institution record for ${record.institutionType || record.id}?`
    }
  )
  
  const initialFormData = {
    institutionType: '',
    year196667: '',
    year197071: '',
    year198081: '',
    year199091: '',
    year200001: '',
    year201011: '',
    year201617: '',
    year201718: '',
    year201819: '',
    year201920: '',
    year202021: '',
    year202122: '',
    year202223: '',
    year202324: '',
    year202425: '',
  }
  
  const { dialogOpen, editingRecord, formData, setFormData, openCreate, openEdit, closeDialog, isEditMode } = useFormDialog(
    initialFormData,
    mapToFormData
  )
  
  // Audit History Modal State
  const [showAuditHistory, setShowAuditHistory] = useState(false)
  

  // Listen for screen workflow status changes
  useEffect(() => {
    const handleStatusChange = async (event) => {
      const { screenCode, newStatusId, screenLevel } = event.detail;
      if (screenCode === 'ED_TABLE_6_1_INSTITUTIONS' && screenLevel) {
        await refreshStatus();
        await refreshData();
      }
    };

    window.addEventListener('workflowStatusChanged', handleStatusChange);
    return () => window.removeEventListener('workflowStatusChanged', handleStatusChange);
  }, [refreshStatus, refreshData]);

  // Reload screen status when dialog closes (after workflow actions)
  useEffect(() => {
    if (!dialogOpen) {
      refreshStatus();
    }
  }, [dialogOpen, refreshStatus]);

  // Build dynamic columns for the table
  const columns = [
    {
      key: "institutionType",
      label: "Institution Type",
      sortable: true,
    },
    ...yearColumns.map(yearCol => ({
      key: yearCol.key,
      label: yearCol.label,
      sortable: true,
      render: (value) => value !== null && value !== undefined ? formatNumber(value) : '—',
    })),
  ]

  // ✅ Simplified handlers using generalized hooks
  const handleOpenCreate = () => {
    if (checkAndPreventAction('Adding a new record')) return
    openCreate()
  }

  const handleOpenEdit = async (record) => {
    if (checkAndPreventAction('Editing this record')) return
    try {
      // Fetch latest record from API
      const latestRecord = await table6_1_institutionsApi.getById(record.institutionID)
      const mappedRecord = mapInstitutionRecord(latestRecord)
      openEdit(mappedRecord)
    } catch (error) {
      console.error('Failed to load record:', error)
      // Fallback to using the record passed in
      openEdit(record)
    }
  }

  const handleDelete = async (record) => {
    if (checkAndPreventAction('Deleting this record')) return
    const institutionId = record.institutionID || record.id
    if (!institutionId) {
      alert('Error: InstitutionID not found. Cannot delete record.')
      return
    }
    await deleteRecord(institutionId, record)
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    
    // Build API payload
    const apiData = {
      institutionType: formData.institutionType,
      year196667: formData.year196667 ? parseInt(formData.year196667) : null,
      year197071: formData.year197071 ? parseInt(formData.year197071) : null,
      year198081: formData.year198081 ? parseInt(formData.year198081) : null,
      year199091: formData.year199091 ? parseInt(formData.year199091) : null,
      year200001: formData.year200001 ? parseInt(formData.year200001) : null,
      year201011: formData.year201011 ? parseInt(formData.year201011) : null,
      year201617: formData.year201617 ? parseInt(formData.year201617) : null,
      year201718: formData.year201718 ? parseInt(formData.year201718) : null,
      year201819: formData.year201819 ? parseInt(formData.year201819) : null,
      year201920: formData.year201920 ? parseInt(formData.year201920) : null,
      year202021: formData.year202021 ? parseInt(formData.year202021) : null,
      year202122: formData.year202122 ? parseInt(formData.year202122) : null,
      year202223: formData.year202223 ? parseInt(formData.year202223) : null,
      year202324: formData.year202324 ? parseInt(formData.year202324) : null,
      year202425: formData.year202425 ? parseInt(formData.year202425) : null
    }

    try {
      if (isEditMode && editingRecord) {
        // ✅ Use generalized updateRecord
        const institutionId = editingRecord.institutionID || editingRecord.id
        if (!institutionId) {
          alert('Error: InstitutionID not found. Cannot update record.')
          return
        }
        await updateRecord(institutionId, apiData)
      } else {
        // ✅ Use generalized createRecord
        await createRecord(apiData)
      }
      closeDialog()
    } catch (error) {
      console.error('Failed to save record:', error);
      alert(error.message || 'Failed to save record. Please try again.');
    }
  }

  const handleRowAction = (row) => {
    const actions = []
    
    // Only allow edit if screen is not locked
    if (!isLocked) {
      actions.push({
        label: "Edit",
        onClick: () => handleOpenEdit(row),
      })
    }
    
    // Only allow delete if screen is not locked
    if (!isLocked) {
      actions.push({
        label: "Delete",
        onClick: () => handleDelete(row),
        destructive: true,
      })
    }
    
    // If locked, add a disabled info action
    if (isLocked && actions.length === 0) {
      actions.push({
        label: lockedMessage || "Screen is locked",
        onClick: () => {},
        disabled: true,
      })
    }
    
    return actions
  }

  // ✅ Use generalized export utility
  const handleExport = () => {
    const exportColumns = [
      { key: 'institutionType', label: 'Institution Type' },
      ...yearColumns.map(yc => ({ key: yc.key, label: yc.label }))
    ]
    exportToCSV(data, exportColumns, 'education_institutions_data')
  }

  return (
    <div className="space-y-6 p-6">
      <PageHeader
        title="Institutions Management (Table 6.1)"
        breadcrumbs={["Home", "Education", "Table 6.1"]}
        description="Manage number of recognised universities/colleges/schools in Haryana"
        primaryAction={{
          label: "Add Institution",
          icon: Plus,
          onClick: handleOpenCreate,
          disabled: isLocked,
          tooltip: isLocked ? lockedMessage : undefined,
        }}
        secondaryActions={[
          {
            label: "Audit History",
            icon: History,
            variant: "outline",
            onClick: () => setShowAuditHistory(true),
          },
          {
            label: "Export CSV",
            icon: Download,
            variant: "outline",
            onClick: handleExport,
          },
        ]}
      />


      {/* Workflow Status Bar - Screen Level: Always visible when there's at least one record */}
      {data.length > 0 && !loading && (
        <div className="mb-4">
          <WorkflowStatusBar
            screenCode="ED_TABLE_6_1_INSTITUTIONS"
            currentStatusId={screenWorkflowStatus}
            onStatusChange={async (newStatusId, remarks = null) => {
              // Refresh workflow status from the hook
              await refreshStatus();
              // Screen-level workflow is handled by the backend, no need to update individual records
              // Optionally reload data if needed
              await loadData();
            }}
          />
        </div>
      )}

      {/* Data Table */}
      <div className="space-y-4">
        {loading ? (
          <div className="flex items-center justify-center py-8">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
            <span className="ml-2">Loading data...</span>
          </div>
        ) : (
          <DataTable
            columns={columns}
            data={data}
            searchable={true}
            selectable={true}
            onRowAction={handleRowAction}
          />
        )}
      </div>

      {/* Create/Edit Dialog */}
      <Dialog open={dialogOpen} onOpenChange={(open) => {
        if (!open) {
          closeDialog();
        }
      }}>
        <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>{editingRecord ? 'Edit Institution Record' : 'Add New Institution Record'}</DialogTitle>
            <DialogDescription>
              {editingRecord 
                ? 'Update the institution data for the selected type.' 
                : 'Enter the institution data for a new type.'}
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={handleSubmit}>
            <div className="grid gap-4 py-4">
              <div className="space-y-2">
                <Label htmlFor="institutionType">Institution Type *</Label>
                <Input
                  id="institutionType"
                  value={formData.institutionType}
                  onChange={(e) => setFormData({ ...formData, institutionType: e.target.value })}
                  required
                  disabled={isEditMode}
                  placeholder="e.g., Universities, Arts and Science Colleges"
                />
              </div>

              {/* Year columns in a grid */}
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
                {yearColumns.map((yearCol) => (
                  <div key={yearCol.key} className="space-y-2">
                    <Label htmlFor={yearCol.key}>{yearCol.label}</Label>
                    <Input
                      id={yearCol.key}
                      type="number"
                      min="0"
                      value={formData[yearCol.key]}
                      onChange={(e) => setFormData({ ...formData, [yearCol.key]: e.target.value })}
                      placeholder="—"
                    />
                  </div>
                ))}
              </div>
            </div>
            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => closeDialog()}>
                Cancel
              </Button>
              <Button type="submit">
                {editingRecord ? 'Update Record' : 'Create Record'}
              </Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      {/* Audit History Modal */}
      <ScreenAuditHistoryModal
        open={showAuditHistory}
        onOpenChange={setShowAuditHistory}
        screenCode="ED_TABLE_6_1_INSTITUTIONS"
        screenName="Institutions Management (Table 6.1)"
        currentStatusId={screenWorkflowStatus}
        onWorkflowReset={async () => {
          // Refresh workflow status after reset
          await refreshStatus();
          // Reload data
          await refreshData();
        }}
      />
    </div>
  )
}
