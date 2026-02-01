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
import table7_6_noOfPrisonersClasswiseApi from "@/services/SocialSecurityAndSocialDefence/table7_6_noOfPrisonersClasswiseApi"
import { useAuth } from "@/contexts/AuthContext"
import { useWorkflowLock } from "@/hooks/useWorkflowLock"
import { useCrudOperations } from "@/hooks/useCrudOperations"
import { useFormDialog } from "@/hooks/useFormDialog"
import { exportToCSV } from "@/utils/export"
import { formatNumber } from "@/utils/format"

const getVal = (r, ...keys) => { for (const k of keys) { const v = r?.[k]; if (v !== undefined && v !== null) return v } return undefined }
const mapRecord = (record) => ({
  id: record.id, year: record.year,
  beg_convicted: getVal(record, 'beg_Convicted', 'beg_convicted'),
  beg_undertrial: getVal(record, 'beg_UnderTrial', 'beg_undertrial'),
  beg_civil: getVal(record, 'beg_Civil', 'beg_civil'),
  adm_convicted: getVal(record, 'adm_Convicted', 'adm_convicted'),
  adm_undertrial: getVal(record, 'adm_UnderTrial', 'adm_undertrial'),
  adm_civil: getVal(record, 'adm_Civil', 'adm_civil'),
  dis_convicted: getVal(record, 'dis_Convicted', 'dis_convicted'),
  dis_undertrial: getVal(record, 'dis_UnderTrial', 'dis_undertrial'),
  dis_civil: getVal(record, 'dis_Civil', 'dis_civil'),
  rem_convicted: getVal(record, 'rem_Convicted', 'rem_convicted'),
  rem_undertrial: getVal(record, 'rem_UnderTrial', 'rem_undertrial'),
  rem_civil: getVal(record, 'rem_Civil', 'rem_civil'),
})
const mapToFormData = (record) => ({
  year: record?.year ?? '',
  beg_convicted: record?.beg_convicted ?? record?.beg_Convicted ?? '',
  beg_undertrial: record?.beg_undertrial ?? record?.beg_UnderTrial ?? '',
  beg_civil: record?.beg_civil ?? record?.beg_Civil ?? '',
  adm_convicted: record?.adm_convicted ?? record?.adm_Convicted ?? '',
  adm_undertrial: record?.adm_undertrial ?? record?.adm_UnderTrial ?? '',
  adm_civil: record?.adm_civil ?? record?.adm_Civil ?? '',
  dis_convicted: record?.dis_convicted ?? record?.dis_Convicted ?? '',
  dis_undertrial: record?.dis_undertrial ?? record?.dis_UnderTrial ?? '',
  dis_civil: record?.dis_civil ?? record?.dis_Civil ?? '',
})
const calcRem = (beg, adm, dis) => Math.max(0, (parseInt(beg) || 0) + (parseInt(adm) || 0) - (parseInt(dis) || 0))

export default function Table7_6_NoOfPrisonersClasswise() {
  const { user } = useAuth()
  const { isLocked, statusId: screenWorkflowStatus, lockedMessage, checkAndPreventAction, refreshStatus } = useWorkflowLock('SSD_TABLE_7_6_NO_OF_PRISONERS_CLASSWISE')
  const { data, loading, error, createRecord, updateRecord, deleteRecord, refreshData, loadData, setError } = useCrudOperations(table7_6_noOfPrisonersClasswiseApi, mapRecord, { autoLoad: true, deleteMessage: (r) => `Delete record for year ${r.year || r.id}?` })
  const initialFormData = { year: '', beg_convicted: '', beg_undertrial: '', beg_civil: '', adm_convicted: '', adm_undertrial: '', adm_civil: '', dis_convicted: '', dis_undertrial: '', dis_civil: '' }
  const { dialogOpen, editingRecord, formData, setFormData, openCreate, openEdit, closeDialog, isEditMode } = useFormDialog(initialFormData, mapToFormData)
  const [showAuditHistory, setShowAuditHistory] = useState(false)

  useEffect(() => { const h = async (e) => { if (e.detail?.screenCode === 'SSD_TABLE_7_6_NO_OF_PRISONERS_CLASSWISE' && e.detail?.screenLevel) { await refreshStatus(); await refreshData() } }; window.addEventListener('workflowStatusChanged', h); return () => window.removeEventListener('workflowStatusChanged', h) }, [refreshStatus, refreshData])
  useEffect(() => { if (!dialogOpen) refreshStatus() }, [dialogOpen, refreshStatus])

  const columns = [
    { key: "year", label: "Year", sortable: true },
    { key: "beg_convicted", label: "Beg: Convicted", sortable: true, render: v => formatNumber(v) },
    { key: "beg_undertrial", label: "Beg: Under trial", sortable: true, render: v => formatNumber(v) },
    { key: "beg_civil", label: "Beg: Civil*", sortable: true, render: v => formatNumber(v) },
    { key: "adm_convicted", label: "Adm: Convicted", sortable: true, render: v => formatNumber(v) },
    { key: "adm_undertrial", label: "Adm: Under trial", sortable: true, render: v => formatNumber(v) },
    { key: "adm_civil", label: "Adm: Civil*", sortable: true, render: v => formatNumber(v) },
    { key: "dis_convicted", label: "Dis: Convicted", sortable: true, render: v => formatNumber(v) },
    { key: "dis_undertrial", label: "Dis: Under trial", sortable: true, render: v => formatNumber(v) },
    { key: "dis_civil", label: "Dis: Civil*", sortable: true, render: v => formatNumber(v) },
    { key: "rem_convicted", label: "Rem: Convicted", sortable: true, render: v => formatNumber(v) },
    { key: "rem_undertrial", label: "Rem: Under trial", sortable: true, render: v => formatNumber(v) },
    { key: "rem_civil", label: "Rem: Civil*", sortable: true, render: v => formatNumber(v) },
  ]

  const handleOpenCreate = () => { if (checkAndPreventAction('Adding record')) return; openCreate() }
  const handleOpenEdit = async (record) => {
    if (checkAndPreventAction('Editing')) return
    try { openEdit(mapRecord(await table7_6_noOfPrisonersClasswiseApi.getByYear(record.year))) } catch { openEdit(record) }
  }
  const handleDelete = async (record) => { if (checkAndPreventAction('Deleting')) return; await deleteRecord(record.id, record) }
  const handleSubmit = async (e) => {
    e.preventDefault()
    const year = String(formData.year || '').trim()
    if (!year) { alert('Year is required'); return }
    const apiData = {
      year,
      beg_Convicted: parseInt(formData.beg_convicted) || 0, beg_UnderTrial: parseInt(formData.beg_undertrial) || 0, beg_Civil: parseInt(formData.beg_civil) || 0,
      adm_Convicted: parseInt(formData.adm_convicted) || 0, adm_UnderTrial: parseInt(formData.adm_undertrial) || 0, adm_Civil: parseInt(formData.adm_civil) || 0,
      dis_Convicted: parseInt(formData.dis_convicted) || 0, dis_UnderTrial: parseInt(formData.dis_undertrial) || 0, dis_Civil: parseInt(formData.dis_civil) || 0,
    }
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
  const handleExport = () => exportToCSV(data, columns.map(c => ({ key: c.key, label: c.label })), 'table7_6_prisoners')

  const remConvicted = calcRem(formData.beg_convicted, formData.adm_convicted, formData.dis_convicted)
  const remUndertrial = calcRem(formData.beg_undertrial, formData.adm_undertrial, formData.dis_undertrial)
  const remCivil = calcRem(formData.beg_civil, formData.adm_civil, formData.dis_civil)

  return (
    <div className="space-y-6 p-6">
      <PageHeader title="SOCIAL SECURITY AND SOCIAL DEFENCE" breadcrumbs={["Home", "SOCIAL SECURITY AND SOCIAL DEFENCE"]} description="Table 7.6 Number of prisoners (class-wise) in Haryana."
        primaryAction={{ label: "Add Record", icon: Plus, onClick: handleOpenCreate, disabled: isLocked, tooltip: isLocked ? lockedMessage : undefined }}
        secondaryActions={[{ label: "Audit History", icon: History, variant: "outline", onClick: () => setShowAuditHistory(true) }, { label: "Export CSV", icon: Download, variant: "outline", onClick: handleExport }]} />

      {data.length > 0 && !loading && <div className="mb-4"><WorkflowStatusBar screenCode="SSD_TABLE_7_6_NO_OF_PRISONERS_CLASSWISE" currentStatusId={screenWorkflowStatus} onStatusChange={async () => { await refreshStatus(); await loadData() }} /></div>}

      <div className="space-y-4">
        {loading ? (
          <div className="flex justify-center py-8"><div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900" /><span className="ml-2">Loading...</span></div>
        ) : error ? (
          <div className="rounded-lg border border-amber-200 bg-amber-50 p-4"><p>{error}</p><Button variant="outline" size="sm" className="mt-2" onClick={() => { setError(null); loadData() }}>Retry</Button></div>
        ) : (
          <DataTable columns={columns} data={data || []} searchable selectable onRowAction={handleRowAction} />
        )}
      </div>

      <div className="rounded-lg border border-slate-200 bg-slate-50 p-4 text-sm text-slate-700">
        <p className="font-medium mb-2">Source:</p>
        <p className="mb-3">Director General of Prisons, Haryana.</p>
        <p className="font-medium mb-2">Note:</p>
        <ul className="list-disc list-inside space-y-1 mb-3">
          <li>This table includes prisoners in Sudharghars, Sub-Sudharghars and Judicial lock-ups in Haryana.</li>
          <li>Information for 1990-91 and onwards relates to financial year.</li>
        </ul>
        <p><span className="font-medium">*</span> : Detenues</p>
      </div>

      <Dialog open={dialogOpen} onOpenChange={o => { if (!o) closeDialog() }}>
        <DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto">
          <DialogHeader><DialogTitle>{editingRecord ? 'Edit' : 'Add'} Record</DialogTitle><DialogDescription>Prisoners class-wise data.</DialogDescription></DialogHeader>
          <form onSubmit={handleSubmit}>
            <div className="grid gap-4 py-4">
              <div><Label>Year *</Label><Input type="text" placeholder="e.g. 1966 or 1990-91" value={formData.year} onChange={e => setFormData({ ...formData, year: e.target.value })} required /></div>
              <div className="grid grid-cols-3 gap-4">
                <div><Label>Beg: Convicted</Label><Input type="number" min="0" value={formData.beg_convicted} onChange={e => setFormData({ ...formData, beg_convicted: e.target.value })} /></div>
                <div><Label>Beg: Under trial</Label><Input type="number" min="0" value={formData.beg_undertrial} onChange={e => setFormData({ ...formData, beg_undertrial: e.target.value })} /></div>
                <div><Label>Beg: Civil*</Label><Input type="number" min="0" value={formData.beg_civil} onChange={e => setFormData({ ...formData, beg_civil: e.target.value })} /></div>
              </div>
              <div className="grid grid-cols-3 gap-4">
                <div><Label>Adm: Convicted</Label><Input type="number" min="0" value={formData.adm_convicted} onChange={e => setFormData({ ...formData, adm_convicted: e.target.value })} /></div>
                <div><Label>Adm: Under trial</Label><Input type="number" min="0" value={formData.adm_undertrial} onChange={e => setFormData({ ...formData, adm_undertrial: e.target.value })} /></div>
                <div><Label>Adm: Civil*</Label><Input type="number" min="0" value={formData.adm_civil} onChange={e => setFormData({ ...formData, adm_civil: e.target.value })} /></div>
              </div>
              <div className="grid grid-cols-3 gap-4">
                <div><Label>Dis: Convicted</Label><Input type="number" min="0" value={formData.dis_convicted} onChange={e => setFormData({ ...formData, dis_convicted: e.target.value })} /></div>
                <div><Label>Dis: Under trial</Label><Input type="number" min="0" value={formData.dis_undertrial} onChange={e => setFormData({ ...formData, dis_undertrial: e.target.value })} /></div>
                <div><Label>Dis: Civil*</Label><Input type="number" min="0" value={formData.dis_civil} onChange={e => setFormData({ ...formData, dis_civil: e.target.value })} /></div>
              </div>
              <div className="grid grid-cols-3 gap-4">
                <div><Label>Rem: Convicted (read-only)</Label><Input type="number" value={remConvicted} readOnly disabled className="bg-muted" /></div>
                <div><Label>Rem: Under trial (read-only)</Label><Input type="number" value={remUndertrial} readOnly disabled className="bg-muted" /></div>
                <div><Label>Rem: Civil* (read-only)</Label><Input type="number" value={remCivil} readOnly disabled className="bg-muted" /></div>
              </div>
            </div>
            <DialogFooter><Button type="button" variant="outline" onClick={() => closeDialog()}>Cancel</Button><Button type="submit">{editingRecord ? 'Update' : 'Create'}</Button></DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      <ScreenAuditHistoryModal open={showAuditHistory} onOpenChange={setShowAuditHistory} screenCode="SSD_TABLE_7_6_NO_OF_PRISONERS_CLASSWISE" screenName="Table 7.6 Prisoners Class-wise" currentStatusId={screenWorkflowStatus} onWorkflowReset={async () => { await refreshStatus(); await refreshData() }} />
    </div>
  )
}
