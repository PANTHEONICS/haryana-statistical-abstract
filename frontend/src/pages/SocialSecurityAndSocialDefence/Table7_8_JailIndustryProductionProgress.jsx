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
import table7_8_jailIndustryProductionProgressApi from "@/services/SocialSecurityAndSocialDefence/table7_8_jailIndustryProductionProgressApi"
import { useAuth } from "@/contexts/AuthContext"
import { useWorkflowLock } from "@/hooks/useWorkflowLock"
import { useCrudOperations } from "@/hooks/useCrudOperations"
import { useFormDialog } from "@/hooks/useFormDialog"
import { exportToCSV } from "@/utils/export"
import { formatNumber } from "@/utils/format"

const getVal = (r, ...keys) => { for (const k of keys) { const v = r?.[k]; if (v !== undefined && v !== null) return v } return undefined }
const CATEGORY_KEYS = ['carpentry', 'textile', 'leather', 'durries', 'tailoring', 'munj', 'chicks', 'oil_OilCake', 'tents', 'blankets', 'smithy', 'niwar_Tapes', 'misc']
const isEmptyYear = (v) => v == null || v === '' || String(v).toUpperCase() === 'NULL'
const mapRecord = (record) => {
  const yearVal = getVal(record, 'year', 'Year')
  const out = { id: record.id, year: !isEmptyYear(yearVal) ? String(yearVal).trim() : '—' }
  CATEGORY_KEYS.forEach(key => { out[key] = getVal(record, key, key.replace(/_/g, '_')) })
  out.total = getVal(record, 'total', 'Total')
  return out
}
const mapToFormData = (record) => {
  const y = getVal(record, 'year', 'Year')
  const fd = { year: !isEmptyYear(y) ? String(y).trim() : '' }
  CATEGORY_KEYS.forEach(key => { fd[key] = getVal(record, key) !== undefined ? getVal(record, key) : '' })
  return fd
}
const parseNum = (v) => parseInt(String(v).replace(/,/g, ''), 10) || 0
const calculateTotal = (fd) => CATEGORY_KEYS.reduce((sum, key) => sum + parseNum(fd[key]), 0)

export default function Table7_8_JailIndustryProductionProgress() {
  const { user } = useAuth()
  const { isLocked, statusId: screenWorkflowStatus, lockedMessage, checkAndPreventAction, refreshStatus } = useWorkflowLock('SSD_TABLE_7_8_JAIL_INDUSTRY_PRODUCTION_PROGRESS')
  const { data, loading, error, createRecord, updateRecord, deleteRecord, refreshData, loadData, setError } = useCrudOperations(table7_8_jailIndustryProductionProgressApi, mapRecord, { autoLoad: true, deleteMessage: (r) => `Delete record for year ${r.year || r.id}?` })
  const initialFormData = { year: '', ...Object.fromEntries(CATEGORY_KEYS.map(k => [k, ''])) }
  const { dialogOpen, editingRecord, formData, setFormData, openCreate, openEdit, closeDialog, isEditMode } = useFormDialog(initialFormData, mapToFormData)
  const [showAuditHistory, setShowAuditHistory] = useState(false)

  useEffect(() => { const h = async (e) => { if (e.detail?.screenCode === 'SSD_TABLE_7_8_JAIL_INDUSTRY_PRODUCTION_PROGRESS' && e.detail?.screenLevel) { await refreshStatus(); await refreshData() } }; window.addEventListener('workflowStatusChanged', h); return () => window.removeEventListener('workflowStatusChanged', h) }, [refreshStatus, refreshData])
  useEffect(() => { if (!dialogOpen) refreshStatus() }, [dialogOpen, refreshStatus])

  const columnLabels = [
    { key: 'year', label: 'Year' },
    { key: 'carpentry', label: 'Carpentry (₹)' },
    { key: 'textile', label: 'Textile (₹)' },
    { key: 'leather', label: 'Leather (₹)' },
    { key: 'durries', label: 'Durries (₹)' },
    { key: 'tailoring', label: 'Tailoring (₹)' },
    { key: 'munj', label: 'Munj (₹)' },
    { key: 'chicks', label: 'Chicks (₹)' },
    { key: 'oil_OilCake', label: 'Oil & Oil Cake (₹)' },
    { key: 'tents', label: 'Tents (₹)' },
    { key: 'blankets', label: 'Blankets (₹)' },
    { key: 'smithy', label: 'Smithy (₹)' },
    { key: 'niwar_Tapes', label: 'Niwar & Tapes (₹)' },
    { key: 'misc', label: 'Misc (₹)' },
    { key: 'total', label: 'Total (₹)' },
  ]
  const columns = columnLabels.map(({ key, label }) => ({
    key,
    label,
    sortable: true,
    render: key === 'year' ? (v) => (v != null && v !== '' ? String(v) : '—') : (v) => formatNumber(v)
  }))

  const handleOpenCreate = () => { if (checkAndPreventAction('Adding record')) return; openCreate() }
  const handleOpenEdit = async (record) => {
    if (checkAndPreventAction('Editing')) return
    try { openEdit(mapRecord(await table7_8_jailIndustryProductionProgressApi.getByYear(record.year))) } catch { openEdit(record) }
  }
  const handleDelete = async (record) => { if (checkAndPreventAction('Deleting')) return; await deleteRecord(record.id, record) }
  const handleSubmit = async (e) => {
    e.preventDefault()
    const year = String(formData.year || '').trim()
    if (!year) { alert('Year is required'); return }
    const vals = {}
    let hasNegative = false
    CATEGORY_KEYS.forEach(key => { const n = parseNum(formData[key]); if (n < 0) hasNegative = true; vals[key] = n })
    if (hasNegative) { alert('Values cannot be negative'); return }
    const apiData = { year, ...vals }
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
  const handleExport = () => exportToCSV(data, columnLabels, 'table7_8_jail_industry')

  const computedTotal = calculateTotal(formData)

  return (
    <div className="space-y-6 p-6">
      <PageHeader title="SOCIAL SECURITY AND SOCIAL DEFENCE" breadcrumbs={["Home", "SOCIAL SECURITY AND SOCIAL DEFENCE"]} description="Table 7.8 Progress of Jail Industry in Haryana. Value of goods produced."
        primaryAction={{ label: "Add Record", icon: Plus, onClick: handleOpenCreate, disabled: isLocked, tooltip: isLocked ? lockedMessage : undefined }}
        secondaryActions={[{ label: "Audit History", icon: History, variant: "outline", onClick: () => setShowAuditHistory(true) }, { label: "Export CSV", icon: Download, variant: "outline", onClick: handleExport }]} />

      {data.length > 0 && !loading && <div className="mb-4"><WorkflowStatusBar screenCode="SSD_TABLE_7_8_JAIL_INDUSTRY_PRODUCTION_PROGRESS" currentStatusId={screenWorkflowStatus} onStatusChange={async () => { await refreshStatus(); await loadData() }} /></div>}

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
        <DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto">
          <DialogHeader><DialogTitle>{editingRecord ? 'Edit' : 'Add'} Record</DialogTitle><DialogDescription>Value of goods produced (₹).</DialogDescription></DialogHeader>
          <form onSubmit={handleSubmit}>
            <div className="grid gap-4 py-4">
              <div><Label>Year *</Label><Input type="text" placeholder="e.g. 2023-24 or 2023-24(P)" value={formData.year} onChange={e => setFormData({ ...formData, year: e.target.value })} required /></div>
              <div className="grid grid-cols-2 sm:grid-cols-3 gap-4">
                {columnLabels.filter(c => c.key !== 'year' && c.key !== 'total').map(({ key, label }) => (
                  <div key={key}><Label>{label}</Label><Input type="number" min="0" value={formData[key] ?? ''} onChange={e => setFormData({ ...formData, [key]: e.target.value })} /></div>
                ))}
              </div>
              <div><Label>Total (read-only)</Label><Input type="number" value={computedTotal} readOnly disabled className="bg-muted" /></div>
            </div>
            <DialogFooter><Button type="button" variant="outline" onClick={() => closeDialog()}>Cancel</Button><Button type="submit">{editingRecord ? 'Update' : 'Create'}</Button></DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      <ScreenAuditHistoryModal open={showAuditHistory} onOpenChange={setShowAuditHistory} screenCode="SSD_TABLE_7_8_JAIL_INDUSTRY_PRODUCTION_PROGRESS" screenName="Table 7.8 Jail Industry Production Progress" currentStatusId={screenWorkflowStatus} onWorkflowReset={async () => { await refreshStatus(); await refreshData() }} />
    </div>
  )
}
