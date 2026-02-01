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
import table7_1_sanctionedStrengthPoliceApi from "@/services/SocialSecurityAndSocialDefence/table7_1_sanctionedStrengthPoliceApi"
import { useAuth } from "@/contexts/AuthContext"
import { useWorkflowLock } from "@/hooks/useWorkflowLock"
import { useCrudOperations } from "@/hooks/useCrudOperations"
import { useFormDialog } from "@/hooks/useFormDialog"
import { exportToCSV } from "@/utils/export"
import { formatNumber } from "@/utils/format"

const getVal = (r, ...keys) => { for (const k of keys) { const v = r?.[k]; if (v !== undefined && v !== null) return v } return undefined }
const mapRecord = (record) => ({
  id: record.id, year: record.year,
  dg_adg_ig_dyig: getVal(record, 'dg_ADG_IG_DyIG', 'dG_ADG_IG_DyIG', 'dg_adg_ig_dyig'),
  asst_ig: getVal(record, 'asst_IG', 'asst_ig'),
  superintendents_addl_dy_asst: getVal(record, 'superintendents_Addl_Dy_Asst', 'superintendents_addl_dy_asst'),
  inspectors_si_asi: getVal(record, 'inspectors_SI_ASI', 'inspectors_si_asi'),
  head_constables_rc: getVal(record, 'head_Constables_RC', 'head_constables_rc'),
  mounted_foot_constables: getVal(record, 'mounted_Foot_Constables', 'mounted_foot_constables'),
  total: record.total,
})
const mapToFormData = (record) => ({
  year: record?.year ?? '', dg_adg_ig_dyig: record?.dg_adg_ig_dyig ?? record?.dg_ADG_IG_DyIG ?? '',
  asst_ig: record?.asst_ig ?? record?.asst_IG ?? '',
  superintendents_addl_dy_asst: record?.superintendents_addl_dy_asst ?? record?.superintendents_Addl_Dy_Asst ?? '',
  inspectors_si_asi: record?.inspectors_si_asi ?? record?.inspectors_SI_ASI ?? '',
  head_constables_rc: record?.head_constables_rc ?? record?.head_Constables_RC ?? '',
  mounted_foot_constables: record?.mounted_foot_constables ?? record?.mounted_Foot_Constables ?? '',
  total: record?.total ?? '',
})
const calculateTotal = (fd) => (parseInt(fd.dg_adg_ig_dyig)||0)+(parseInt(fd.asst_ig)||0)+(parseInt(fd.superintendents_addl_dy_asst)||0)+(parseInt(fd.inspectors_si_asi)||0)+(parseInt(fd.head_constables_rc)||0)+(parseInt(fd.mounted_foot_constables)||0)

export default function Table7_1_SanctionedStrengthPolice() {
  const { user } = useAuth()
  const { isLocked, statusId: screenWorkflowStatus, lockedMessage, checkAndPreventAction, refreshStatus } = useWorkflowLock('SSD_TABLE_7_1_SANCTIONED_STRENGTH_POLICE')
  const { data, loading, error, createRecord, updateRecord, deleteRecord, refreshData, loadData, setError } = useCrudOperations(table7_1_sanctionedStrengthPoliceApi, mapRecord, { autoLoad: true, deleteMessage: (r) => `Delete record for year ${r.year || r.id}?` })
  const initialFormData = { year: '', dg_adg_ig_dyig: '', asst_ig: '', superintendents_addl_dy_asst: '', inspectors_si_asi: '', head_constables_rc: '', mounted_foot_constables: '', total: '' }
  const { dialogOpen, editingRecord, formData, setFormData, openCreate, openEdit, closeDialog, isEditMode } = useFormDialog(initialFormData, mapToFormData)
  const [showAuditHistory, setShowAuditHistory] = useState(false)

  useEffect(() => { const h = async (e) => { if (e.detail?.screenCode === 'SSD_TABLE_7_1_SANCTIONED_STRENGTH_POLICE' && e.detail?.screenLevel) { await refreshStatus(); await refreshData() } }; window.addEventListener('workflowStatusChanged', h); return () => window.removeEventListener('workflowStatusChanged', h) }, [refreshStatus, refreshData])
  useEffect(() => { if (!dialogOpen) refreshStatus() }, [dialogOpen, refreshStatus])

  const columns = [
    { key: "year", label: "Year", sortable: true },
    { key: "dg_adg_ig_dyig", label: "DG/ADG/IG/DyIG", sortable: true, render: v => formatNumber(v) },
    { key: "asst_ig", label: "Asst. IG", sortable: true, render: v => formatNumber(v) },
    { key: "superintendents_addl_dy_asst", label: "Superintendents (Addl/Dy/Asst)", sortable: true, render: v => formatNumber(v) },
    { key: "inspectors_si_asi", label: "Inspectors (SI/ASI)", sortable: true, render: v => formatNumber(v) },
    { key: "head_constables_rc", label: "Head Constables (RC)", sortable: true, render: v => formatNumber(v) },
    { key: "mounted_foot_constables", label: "Mounted/Foot Constables", sortable: true, render: v => formatNumber(v) },
    { key: "total", label: "Total", sortable: true, render: v => formatNumber(v) },
  ]

  const handleOpenCreate = () => { if (checkAndPreventAction('Adding record')) return; openCreate() }
  const handleOpenEdit = async (record) => {
    if (checkAndPreventAction('Editing')) return
    try { openEdit(mapRecord(await table7_1_sanctionedStrengthPoliceApi.getByYear(record.year))) } catch { openEdit(record) }
  }
  const handleDelete = async (record) => { if (checkAndPreventAction('Deleting')) return; await deleteRecord(record.id, record) }
  const handleSubmit = async (e) => {
    e.preventDefault()
    const year = parseInt(formData.year)
    if (isNaN(year) || year < 1900 || year > 2100) { alert('Year must be 1900-2100'); return }
    const apiData = { year, dg_ADG_IG_DyIG: parseInt(formData.dg_adg_ig_dyig)||0, asst_IG: parseInt(formData.asst_ig)||0, superintendents_Addl_Dy_Asst: parseInt(formData.superintendents_addl_dy_asst)||0, inspectors_SI_ASI: parseInt(formData.inspectors_si_asi)||0, head_Constables_RC: parseInt(formData.head_constables_rc)||0, mounted_Foot_Constables: parseInt(formData.mounted_foot_constables)||0 }
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
  const handleExport = () => exportToCSV(data, columns.map(c=>({key:c.key,label:c.label})), 'table7_1_police')

  return (
    <div className="space-y-6 p-6">
      <PageHeader title="SOCIAL SECURITY AND SOCIAL DEFENCE" breadcrumbs={["Home", "SOCIAL SECURITY AND SOCIAL DEFENCE"]} description="Table 7.1 Information relating to the sanctioned strength of Haryana Police."
        primaryAction={{ label: "Add Record", icon: Plus, onClick: handleOpenCreate, disabled: isLocked, tooltip: isLocked ? lockedMessage : undefined }}
        secondaryActions={[{ label: "Audit History", icon: History, variant: "outline", onClick: () => setShowAuditHistory(true) }, { label: "Export CSV", icon: Download, variant: "outline", onClick: handleExport }]} />

      {data.length > 0 && !loading && <div className="mb-4"><WorkflowStatusBar screenCode="SSD_TABLE_7_1_SANCTIONED_STRENGTH_POLICE" currentStatusId={screenWorkflowStatus} onStatusChange={async () => { await refreshStatus(); await loadData() }} /></div>}

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
          <DialogHeader><DialogTitle>{editingRecord ? 'Edit' : 'Add'} Record</DialogTitle><DialogDescription>Sanctioned strength data.</DialogDescription></DialogHeader>
          <form onSubmit={handleSubmit}>
            <div className="grid gap-4 py-4">
              <div><Label>Year *</Label><Input type="number" value={formData.year} onChange={e=>setFormData({...formData,year:e.target.value})} required min="1900" max="2100" /></div>
              <div className="grid grid-cols-2 gap-4">
                <div><Label>DG/ADG/IG/DyIG</Label><Input type="number" min="0" value={formData.dg_adg_ig_dyig} onChange={e=>setFormData({...formData,dg_adg_ig_dyig:e.target.value})} /></div>
                <div><Label>Asst. IG</Label><Input type="number" min="0" value={formData.asst_ig} onChange={e=>setFormData({...formData,asst_ig:e.target.value})} /></div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div><Label>Superintendents (Addl/Dy/Asst)</Label><Input type="number" min="0" value={formData.superintendents_addl_dy_asst} onChange={e=>setFormData({...formData,superintendents_addl_dy_asst:e.target.value})} /></div>
                <div><Label>Inspectors (SI/ASI)</Label><Input type="number" min="0" value={formData.inspectors_si_asi} onChange={e=>setFormData({...formData,inspectors_si_asi:e.target.value})} /></div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div><Label>Head Constables (RC)</Label><Input type="number" min="0" value={formData.head_constables_rc} onChange={e=>setFormData({...formData,head_constables_rc:e.target.value})} /></div>
                <div><Label>Mounted/Foot Constables</Label><Input type="number" min="0" value={formData.mounted_foot_constables} onChange={e=>setFormData({...formData,mounted_foot_constables:e.target.value})} /></div>
              </div>
              <div><Label>Total (read-only)</Label><Input type="number" value={calculateTotal(formData)} readOnly disabled className="bg-muted" /></div>
            </div>
            <DialogFooter><Button type="button" variant="outline" onClick={()=>closeDialog()}>Cancel</Button><Button type="submit">{editingRecord?'Update':'Create'}</Button></DialogFooter>
          </form>
        </DialogContent>
      </Dialog>

      <ScreenAuditHistoryModal open={showAuditHistory} onOpenChange={setShowAuditHistory} screenCode="SSD_TABLE_7_1_SANCTIONED_STRENGTH_POLICE" screenName="Table 7.1 Sanctioned Strength of Police" currentStatusId={screenWorkflowStatus} onWorkflowReset={async ()=>{await refreshStatus();await refreshData()}} />
    </div>
  )
}
