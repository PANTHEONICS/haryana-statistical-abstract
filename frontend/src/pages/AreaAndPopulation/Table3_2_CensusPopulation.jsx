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
import { Download, Plus, Users, Calendar, History } from "lucide-react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import WorkflowStatusBar from "@/components/workflow/WorkflowStatusBar"
import ScreenAuditHistoryModal from "@/components/workflow/ScreenAuditHistoryModal"
import table3_2_censusPopulationApi from "@/services/AreaAndPopulation/table3_2_censusPopulationApi"
import { useAuth } from "@/contexts/AuthContext"
import { useWorkflowLock } from "@/hooks/useWorkflowLock"
import { useCrudOperations } from "@/hooks/useCrudOperations"
import { useFormDialog } from "@/hooks/useFormDialog"
import { exportToCSV } from "@/utils/export"
import { formatNumber, formatPercentage } from "@/utils/format"

// Data mapper: Transform API response to frontend format
const mapCensusRecord = (record) => ({
  id: record.censusID || record.year,
  censusID: record.censusID,
  year: record.year,
  total_population: record.totalPopulation,
  variation_in_population: record.variationInPopulation,
  decennial_percentage_increase: record.decennialPercentageIncrease,
  male_population: record.malePopulation,
  female_population: record.femalePopulation,
  sex_ratio: record.sexRatio,
})

// Form mapper: Transform record to form data format
const mapToFormData = (record) => ({
  year: record.year || '',
  total_population: record.total_population || record.totalPopulation || '',
  variation_in_population: record.variation_in_population || record.variationInPopulation || '',
  decennial_percentage_increase: record.decennial_percentage_increase || record.decennialPercentageIncrease || '',
  male_population: record.male_population || record.malePopulation || '',
  female_population: record.female_population || record.femalePopulation || '',
  sex_ratio: record.sex_ratio || record.sexRatio || '',
})

export default function Table3_2_CensusPopulation() {
  const { user } = useAuth()
  
  // ✅ Use generalized hooks
  const { isLocked, statusId: screenWorkflowStatus, lockedMessage, checkAndPreventAction, refreshStatus } = useWorkflowLock('AP_TABLE_3_2_CENSUS_POPULATION')
  
  const { data, loading, createRecord, updateRecord, deleteRecord, refreshData, loadData } = useCrudOperations(
    table3_2_censusPopulationApi,
    mapCensusRecord,
    {
      autoLoad: true,
      deleteMessage: (record) => `Are you sure you want to delete the census record for year ${record.year || record.id}?`
    }
  )
  
  const initialFormData = {
    year: '',
    total_population: '',
    variation_in_population: '',
    decennial_percentage_increase: '',
    male_population: '',
    female_population: '',
    sex_ratio: '',
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
      if (screenCode === 'AP_TABLE_3_2_CENSUS_POPULATION' && screenLevel) {
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

  const columns = [
    {
      key: "year",
      label: "Year",
      sortable: true,
    },
    {
      key: "total_population",
      label: "Total Population",
      sortable: true,
      render: (value) => formatNumber(value),
    },
    {
      key: "variation_in_population",
      label: "Variation",
      sortable: true,
      render: (value) => formatNumber(value),
    },
    {
      key: "decennial_percentage_increase",
      label: "Growth %",
      sortable: true,
      render: (value) => formatPercentage(value),
    },
    {
      key: "male_population",
      label: "Male",
      sortable: true,
      render: (value) => formatNumber(value),
    },
    {
      key: "female_population",
      label: "Female",
      sortable: true,
      render: (value) => formatNumber(value),
    },
    {
      key: "sex_ratio",
      label: "Sex Ratio",
      sortable: true,
    },
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
      const latestRecord = await table3_2_censusPopulationApi.getByYear(record.year)
      const mappedRecord = mapCensusRecord(latestRecord)
      openEdit(mappedRecord)
    } catch (error) {
      console.error('Failed to load record:', error)
      // Fallback to using the record passed in
      openEdit(record)
    }
  }

  const handleDelete = async (record) => {
    if (checkAndPreventAction('Deleting this record')) return
    const censusId = record.censusID || record.id
    if (!censusId) {
      alert('Error: CensusID not found. Cannot delete record.')
      return
    }
    await deleteRecord(censusId, record)
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    
    // Calculate variation and percentage if not provided
    const year = parseInt(formData.year)
    const totalPop = parseInt(formData.total_population)
    const malePop = parseInt(formData.male_population)
    const femalePop = parseInt(formData.female_population)
    
    // Validate that male + female = total
    if (malePop + femalePop !== totalPop) {
      alert('Male population + Female population must equal Total population')
      return
    }

    // Find previous record for calculation
    const previousRecord = data
      .filter(r => r.year < year)
      .sort((a, b) => b.year - a.year)[0]

    let variation = formData.variation_in_population 
      ? parseInt(formData.variation_in_population) 
      : (previousRecord ? totalPop - previousRecord.total_population : null)

    let percentage = formData.decennial_percentage_increase
      ? parseFloat(formData.decennial_percentage_increase)
      : (previousRecord && previousRecord.total_population > 0
          ? ((variation / previousRecord.total_population) * 100)
          : null)

    // Calculate sex ratio if not provided
    const sexRatio = formData.sex_ratio 
      ? parseInt(formData.sex_ratio)
      : Math.round((femalePop / malePop) * 1000)

    // Build API payload
    const apiData = {
      year: year,
      totalPopulation: totalPop,
      variationInPopulation: variation,
      decennialPercentageIncrease: percentage,
      malePopulation: malePop,
      femalePopulation: femalePop,
      sexRatio: sexRatio
    }

    try {
      if (isEditMode && editingRecord) {
        // ✅ Use generalized updateRecord
        const censusId = editingRecord.censusID || editingRecord.id
        if (!censusId) {
          alert('Error: CensusID not found. Cannot update record.')
          return
        }
        await updateRecord(censusId, apiData)
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
    exportToCSV(data, [
      { key: 'year', label: 'Year' },
      { key: 'total_population', label: 'Total Population' },
      { key: 'variation_in_population', label: 'Variation' },
      { key: 'decennial_percentage_increase', label: 'Growth %' },
      { key: 'male_population', label: 'Male' },
      { key: 'female_population', label: 'Female' },
      { key: 'sex_ratio', label: 'Sex Ratio' }
    ], 'census_data')
  }

  return (
    <div className="space-y-6 p-6">
      <PageHeader
        title="AREA AND POPULATION"
        breadcrumbs={["Home", "AREA AND POPULATION"]}
        description="Growth of population in Haryana: 1901-2011 Census"
          primaryAction={{
          label: "Add Record",
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
            screenCode="AP_TABLE_3_2_CENSUS_POPULATION"
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
        <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>{editingRecord ? 'Edit Census Record' : 'Add New Census Record'}</DialogTitle>
            <DialogDescription>
              {editingRecord 
                ? 'Update the census population data for the selected year.' 
                : 'Enter the census population data for a new year.'}
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={handleSubmit}>
            <div className="grid gap-4 py-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="year">Census Year *</Label>
                  <Input
                    id="year"
                    type="number"
                    value={formData.year}
                    onChange={(e) => setFormData({ ...formData, year: e.target.value })}
                    required
                    min="1900"
                    max="2100"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="total_population">Total Population *</Label>
                  <Input
                    id="total_population"
                    type="number"
                    value={formData.total_population}
                    onChange={(e) => setFormData({ ...formData, total_population: e.target.value })}
                    required
                    min="1"
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="male_population">Male Population *</Label>
                  <Input
                    id="male_population"
                    type="number"
                    value={formData.male_population}
                    onChange={(e) => setFormData({ ...formData, male_population: e.target.value })}
                    required
                    min="1"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="female_population">Female Population *</Label>
                  <Input
                    id="female_population"
                    type="number"
                    value={formData.female_population}
                    onChange={(e) => setFormData({ ...formData, female_population: e.target.value })}
                    required
                    min="1"
                  />
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="variation_in_population">Variation in Population</Label>
                  <Input
                    id="variation_in_population"
                    type="number"
                    value={formData.variation_in_population}
                    onChange={(e) => setFormData({ ...formData, variation_in_population: e.target.value })}
                    placeholder="Auto-calculated if empty"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="decennial_percentage_increase">Growth Percentage (%)</Label>
                  <Input
                    id="decennial_percentage_increase"
                    type="number"
                    step="0.01"
                    value={formData.decennial_percentage_increase}
                    onChange={(e) => setFormData({ ...formData, decennial_percentage_increase: e.target.value })}
                    placeholder="Auto-calculated if empty"
                  />
                </div>
              </div>
              <div className="space-y-2">
                <Label htmlFor="sex_ratio">Sex Ratio (Females per 1000 males)</Label>
                <Input
                  id="sex_ratio"
                  type="number"
                  value={formData.sex_ratio}
                  onChange={(e) => setFormData({ ...formData, sex_ratio: e.target.value })}
                  placeholder="Auto-calculated if empty"
                />
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
        screenCode="AP_TABLE_3_2_CENSUS_POPULATION"
        screenName="AREA AND POPULATION"
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
