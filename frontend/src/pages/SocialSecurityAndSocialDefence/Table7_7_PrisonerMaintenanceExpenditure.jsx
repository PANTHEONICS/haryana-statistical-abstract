import { useState, useEffect } from "react"
import PageHeader from "@/components/layout/PageHeader"
import { DataTable } from "@/components/ui/DataTable"
import { Button } from "@/components/ui/button"
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Download, Plus, History } from "lucide-react"
import WorkflowStatusBar from "@/components/workflow/WorkflowStatusBar"
import ScreenAuditHistoryModal from "@/components/workflow/ScreenAuditHistoryModal"
import table7_7_prisonerMaintenanceExpenditureApi from "@/services/SocialSecurityAndSocialDefence/table7_7_prisonerMaintenanceExpenditureApi"
import { useAuth } from "@/contexts/AuthContext"
import { useWorkflowLock } from "@/hooks/useWorkflowLock"
import { useCrudOperations } from "@/hooks/useCrudOperations"
import { useFormDialog } from "@/hooks/useFormDialog"
import { exportToCSV } from "@/utils/export"
import { formatNumber } from "@/utils/format"

const getVal = (r, ...keys) => { for (const k of keys) { const v = r?.[k]; if (v !== undefined && v !== null) return v } return undefined }
const mapRecord = (record) => {
  const y = record?.year
  return {
  id: record.id, year: (y != null && y !== '') ? String(y) : '—',
  avg_prisoners: getVal(record, 'avg_Prisoners', 'avg_prisoners'),
  exp_establishment: getVal(record, 'exp_Establishment', 'exp_establishment'),
  exp_diet: getVal(record, 'exp_Diet', 'exp_diet'),
  exp_others: getVal(record, 'exp_Others', 'exp_others'),
  exp_total: getVal(record, 'exp_Total', 'exp_total'),
  cost_per_prisoner: getVal(record, 'cost_Per_Prisoner', 'cost_per_prisoner'),
  }
}
const mapToFormData = (record) => ({
  year: record?.year ?? '',
  avg_prisoners: record?.avg_prisoners ?? record?.avg_Prisoners ?? '',
  exp_establishment: record?.exp_establishment ?? record?.exp_Establishment ?? '',
  exp_diet: record?.exp_diet ?? record?.exp_Diet ?? '',
  exp_others: record?.exp_others ?? record?.exp_Others ?? '',
})
const calcExpTotal = (fd) => (parseInt(fd.exp_establishment) || 0) + (parseInt(fd.exp_diet) || 0) + (parseInt(fd.exp_others) || 0)
const calcCostPerPrisoner = (fd) => { const avg = parseInt(fd.avg_prisoners) || 0; const total = calcExpTotal(fd); return avg > 0 ? Math.floor(total / avg) : 0 }

export default function Table7_7_PrisonerMaintenanceExpenditure() {
  const { user } = useAuth()
  const { isLocked, statusId: screenWorkflowStatus, lockedMessage, checkAndPreventAction, refreshStatus } = useWorkflowLock('SSD_TABLE_7_7_PRISONER_MAINTENANCE_EXPENDITURE')
  const { data, loading, error, createRecord, updateRecord, deleteRecord, refreshData, loadData, setError } = useCrudOperations(table7_7_prisonerMaintenanceExpenditureApi, mapRecord, { autoLoad: true, deleteMessage: (r) => `Delete record for year ${r.year || r.id}?` })
  const initialFormData = { year: '', avg_prisoners: '', exp_establishment: '', exp_diet: '', exp_others: '' }
  const { dialogOpen, editingRecord, formData, setFormData, openCreate, openEdit, closeDialog, isEditMode } = useFormDialog(initialFormData, mapToFormData)
  const [showAuditHistory, setShowAuditHistory] = useState(false)

  useEffect(() => { const h = async (e) => { if (e.detail?.screenCode === 'SSD_TABLE_7_7_PRISONER_MAINTENANCE_EXPENDITURE' && e.detail?.screenLevel) { await refreshStatus(); await refreshData() } }; window.addEventListener('workflowStatusChanged', h); return () => window.removeEventListener('workflowStatusChanged', h) }, [refreshStatus, refreshData])
  useEffect(() => { if (!dialogOpen) refreshStatus() }, [dialogOpen, refreshStatus])

  const columns = [
    { key: "year", label: "Year", sortable: true },
    { key: "avg_prisoners", label: "Avg. Prisoners", sortable: true, render: v => formatNumber(v) },
    { key: "exp_establishment", label: "Exp: Establishment (₹)", sortable: true, render: v => formatNumber(v) },
    { key: "exp_diet", label: "Exp: Diet (₹)", sortable: true, render: v => formatNumber(v) },
    { key: "exp_others", label: "Exp: Others (₹)", sortable: true, render: v => formatNumber(v) },
    { key: "exp_total", label: "Total Expenditure (₹)", sortable: true, render: v => formatNumber(v) },
    { key: "cost_per_prisoner", label: "Cost per Prisoner (₹)", sortable: true, render: v => formatNumber(v) },
  ]

  const handleOpenCreate = () => { if (checkAndPreventAction('Adding record')) return; openCreate() }
  const handleOpenEdit = async (record) => {
    if (checkAndPreventAction('Editing')) return
    try { openEdit(mapRecord(await table7_7_prisonerMaintenanceExpenditureApi.getByYear(record.year))) } catch { openEdit(record) }
  }
  const handleDelete = async (record) => { if (checkAndPreventAction('Deleting')) return; await deleteRecord(record.id, record) }
  const handleSubmit = async (e) => {
    e.preventDefault()
    const year = String(formData.year || '').trim()
    const avgPrisoners = parseInt(formData.avg_prisoners) || 0
    if (!year) { alert('Year is required'); return }
    if (avgPrisoners <= 0) { alert('Daily average number of prisoners must be greater than 0'); return }
    const expEst = parseInt(formData.exp_establishment) || 0
    const expDiet = parseInt(formData.exp_diet) || 0
    const expOthers = parseInt(formData.exp_others) || 0
    if (expEst < 0 || expDiet < 0 || expOthers < 0) { alert('Expenditure values cannot be negative'); return }
    const apiData = { year, avg_Prisoners: avgPrisoners, exp_Establishment: expEst, exp_Diet: expDiet, exp_Others: expOthers }
    try {
      if (isEditMode && editingRecord) await updateRecord(editingRecord.id, apiData)
      else await createRecord(apiData)
      closeDialog()
    } catch (err) { alert(err.message || 'Save failed') }
  }
  const handleRowAction = (row) => {
    if (isLocked) return [{ label: lockedMessage || "Locked", onClick: () => {}, disabled: true }]
    return [
      { label: "Edit", onClick: () => handleOpenEdit(row) },
      { label: "Delete", onClick: () => handleDelete(row), destructive: true }
    ]
  }
  const handleExport = () => exportToCSV(data, columns.map(c => ({ key: c.key, label: c.label })), 'table7_7_expenditure')

  const expTotal = calcExpTotal(formData)
  const costPerPrisoner = calcCostPerPrisoner(formData)

  return (
    <div className="space-y-6 p-6">
      <PageHeader title="SOCIAL SECURITY AND SOCIAL DEFENCE" breadcrumbs={["Home", "SOCIAL SECURITY AND SOCIAL DEFENCE"]} description="Table 7.7 Expenditure on maintenance of prisoners in jails/subsidiary jails* in Haryana."
        primaryAction={{ label: "Add Record", icon: Plus, onClick: handleOpenCreate, disabled: isLocked, tooltip: isLocked ? lockedMessage : undefined }}
        secondaryActions={[{ label: "Audit History", icon: History, variant: "outline", onClick: () => setShowAuditHistory(true) }, { label: "Export CSV", icon: Download, variant: "outline", onClick: handleExport }]} />

      {data.length > 0 && !loading && <div className="mb-4"><WorkflowStatusBar screenCode="SSD_TABLE_7_7_PRISONER_MAINTENANCE_EXPENDITURE" currentStatusId={screenWorkflowStatus} onStatusChange={async () => { await refreshStatus(); await loadData() }} /></div>}

      <div className="space-y-4">
        {loading ? (
          <div className="flex justify-center py-8"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900" /><span className="ml-2">Loading...</span></div>
        ) : error ? (
          <div className="rounded-lg border border-amber-200 bg-amber-50 p-4"><p>{error}</p><Button variant="outline" size="sm" className="mt-2" onClick={() => { setError(null); loadData() }}>Retry</Button></div>
        ) : (
          <DataTable columns={columns} data={data || []} searchable selectable onRowAction={handleRowAction} />
        )}
      </div>

      <Dialog open={dialogOpen} onOpenChange={o => { if (!o) closeDialog() }}>
        <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
          <DialogHeader><DialogTitle>{editingRecord ? 'Edit' : 'Add'} Record</DialogTitle><DialogDescription>Prisoner maintenance expenditure data.</DialogDescription></DialogHeader>
          <form onSubmit={handleSubmit}>
            <div className="grid gap-4 py-4">
              <div className="grid grid-cols-2 gap-4">
                <div><Label>Year *</Label><Input type="text" placeholder="e.g. 2023-24 or 2023-24 (P)" value={formData.year} onChange={e => setFormData({ ...formData, year: e.target.value })} required /></div>
                <div><Label>Daily Avg. Prisoners *</Label><Input type="number" min="0" value={formData.avg_prisoners} onChange={e => setFormData({ ...formData, avg_prisoners: e.target.value })} required /></div>
              </div>
              <div><Label>Expenditure: Establishment (₹)</Label><Input type="number" min="0" value={formData.exp_establishment} onChange={e => setFormData({ ...formData, exp_establishment: e.target.value })} /></div>
              <div><Label>Expenditure: Diet (₹)</Label><Input type="number" min="0" value={formData.exp_diet} onChange={e => setFormData({ ...formData, exp_diet: e.target.value })} /></div>
              <div><Label>Expenditure: Others (₹)</Label><Input type="number" min="0" value={formData.exp_others} onChange={e => setFormData({ ...formData, exp_others: e.target.value })} /></div>
              <div className="grid grid-cols-2 gap-4">
                <div><Label>Total Expenditure (read-only)</Label><Input type="number" value={expTotal} readOnly disabled className="bg-muted" /></div>
                <div><Label>Cost per Prisoner (read-only)</Label><Input type="number" value={costPerPrisoner} readOnly disabled className="bg-muted" /></div>
              </div>
            </div>
            <DialogFooter><Button type="button" variant="outline" onClick={() => closeDialog()}>Cancel</Button><Button type="submit">{editingRecord ? 'Update' : 'Create'}</Button></DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      <ScreenAuditHistoryModal open={showAuditHistory} onOpenChange={setShowAuditHistory} screenCode="SSD_TABLE_7_7_PRISONER_MAINTENANCE_EXPENDITURE" screenName="Table 7.7 Prisoner Maintenance Expenditure" currentStatusId={screenWorkflowStatus} onWorkflowReset={async () => { await refreshStatus(); await refreshData() }} />
    </div>
  )
}
