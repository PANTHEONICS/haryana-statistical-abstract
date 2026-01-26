import { useState } from "react"
import PageHeader from "@/components/layout/PageHeader"
import { DataTable } from "@/components/ui/DataTable"
import { Button } from "@/components/ui/button"
import { StatusBadge } from "@/components/ui/StatusBadge"
import { Drawer, DrawerContent, DrawerHeader, DrawerTitle, DrawerTrigger } from "@/components/ui/drawer"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Download, Filter, Trash2, Edit, MoreVertical } from "lucide-react"

const mockData = [
  { id: 1, name: "Item A", category: "Category 1", status: "active", date: "2024-01-15", value: "$1,234" },
  { id: 2, name: "Item B", category: "Category 2", status: "pending", date: "2024-01-14", value: "$2,345" },
  { id: 3, name: "Item C", category: "Category 1", status: "active", date: "2024-01-13", value: "$3,456" },
  { id: 4, name: "Item D", category: "Category 3", status: "inactive", date: "2024-01-12", value: "$4,567" },
  { id: 5, name: "Item E", category: "Category 2", status: "active", date: "2024-01-11", value: "$5,678" },
  { id: 6, name: "Item F", category: "Category 1", status: "pending", date: "2024-01-10", value: "$6,789" },
]

const columns = [
  {
    key: "name",
    label: "Name",
    sortable: true,
  },
  {
    key: "category",
    label: "Category",
    sortable: true,
  },
  {
    key: "status",
    label: "Status",
    render: (value) => <StatusBadge status={value} />,
  },
  {
    key: "date",
    label: "Date",
    sortable: true,
  },
  {
    key: "value",
    label: "Value",
    sortable: true,
  },
]

export default function DataManagement() {
  const [filterOpen, setFilterOpen] = useState(false)

  const handleRowAction = (row, index) => [
    {
      label: "Edit",
      onClick: () => console.log("Edit", row),
    },
    {
      label: "Duplicate",
      onClick: () => console.log("Duplicate", row),
    },
    {
      label: "Delete",
      onClick: () => console.log("Delete", row),
      destructive: true,
    },
  ]

  return (
    <div className="space-y-6 p-6">
      <PageHeader
        title="Data Management"
        breadcrumbs={["Home", "Data Management"]}
        description="Manage and organize your data entries"
        primaryAction={{
          label: "Export",
          icon: Download,
          onClick: () => console.log("Export"),
        }}
        secondaryActions={[
          {
            label: "Filter",
            icon: Filter,
            variant: "outline",
            onClick: () => setFilterOpen(true),
          },
        ]}
      />

      <div className="space-y-4">
        <DataTable
          columns={columns}
          data={mockData}
          searchable={true}
          selectable={true}
          onRowAction={handleRowAction}
        />
      </div>

      <Drawer open={filterOpen} onOpenChange={setFilterOpen}>
        <DrawerContent>
          <DrawerHeader>
            <DrawerTitle>Filter Options</DrawerTitle>
          </DrawerHeader>
          <div className="p-6 space-y-4">
            <div className="space-y-2">
              <Label>Category</Label>
              <Select>
                <SelectTrigger>
                  <SelectValue placeholder="Select category" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Categories</SelectItem>
                  <SelectItem value="cat1">Category 1</SelectItem>
                  <SelectItem value="cat2">Category 2</SelectItem>
                  <SelectItem value="cat3">Category 3</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Status</Label>
              <Select>
                <SelectTrigger>
                  <SelectValue placeholder="Select status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Statuses</SelectItem>
                  <SelectItem value="active">Active</SelectItem>
                  <SelectItem value="pending">Pending</SelectItem>
                  <SelectItem value="inactive">Inactive</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="space-y-2">
              <Label>Date Range</Label>
              <div className="flex gap-2">
                <Input type="date" />
                <Input type="date" />
              </div>
            </div>
            <div className="flex gap-2 pt-4">
              <Button className="flex-1">Apply Filters</Button>
              <Button variant="outline" className="flex-1">Reset</Button>
            </div>
          </div>
        </DrawerContent>
      </Drawer>
    </div>
  )
}
