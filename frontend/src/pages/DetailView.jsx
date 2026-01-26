import { useState } from "react"
import PageHeader from "@/components/layout/PageHeader"
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Timeline } from "@/components/ui/Timeline"
import { Button } from "@/components/ui/button"
import { StatusBadge } from "@/components/ui/StatusBadge"
import { Separator } from "@/components/ui/separator"
import { Edit, Trash2, Share2, Save, X } from "lucide-react"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"

const summaryData = {
  id: "ENT-001",
  name: "Primary Entry",
  status: "active",
  category: "Category A",
  created: "2024-01-15",
  updated: "2024-01-20",
  description: "This is a detailed description of the entry with important information.",
}

const timelineData = [
  {
    title: "Entry Updated",
    description: "Details were modified by user",
    time: "2 hours ago",
    status: "completed",
  },
  {
    title: "Status Changed",
    description: "Status updated to Active",
    time: "1 day ago",
    status: "completed",
  },
  {
    title: "Entry Created",
    description: "New entry was created in the system",
    time: "5 days ago",
    status: "completed",
  },
]

export default function DetailView() {
  const [isEditing, setIsEditing] = useState(false)
  const [formData, setFormData] = useState(summaryData)

  return (
    <div className="space-y-6 p-6">
      <PageHeader
        title={formData.name}
        breadcrumbs={["Home", "Data Management", "Detail"]}
        primaryAction={
          isEditing
            ? {
                label: "Save",
                icon: Save,
                onClick: () => setIsEditing(false),
              }
            : {
                label: "Edit",
                icon: Edit,
                onClick: () => setIsEditing(true),
              }
        }
        secondaryActions={[
          {
            label: "Share",
            icon: Share2,
            variant: "outline",
            onClick: () => console.log("Share"),
          },
          {
            label: "Delete",
            icon: Trash2,
            variant: "destructive",
            onClick: () => console.log("Delete"),
          },
        ]}
      />

      <div className="grid gap-6 lg:grid-cols-3">
        <div className="lg:col-span-2 space-y-6">
          {/* Summary Card */}
          <Card>
            <CardHeader>
              <div className="flex items-center justify-between">
                <div>
                  <CardTitle>Summary</CardTitle>
                  <CardDescription>Overview information</CardDescription>
                </div>
                <StatusBadge status={formData.status} />
              </div>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid gap-4 md:grid-cols-2">
                <div>
                  <Label className="text-muted-foreground">ID</Label>
                  <p className="text-sm font-medium">{formData.id}</p>
                </div>
                <div>
                  <Label className="text-muted-foreground">Category</Label>
                  <p className="text-sm font-medium">{formData.category}</p>
                </div>
                <div>
                  <Label className="text-muted-foreground">Created</Label>
                  <p className="text-sm font-medium">{formData.created}</p>
                </div>
                <div>
                  <Label className="text-muted-foreground">Last Updated</Label>
                  <p className="text-sm font-medium">{formData.updated}</p>
                </div>
              </div>
              <Separator />
              <div>
                <Label className="text-muted-foreground">Description</Label>
                {isEditing ? (
                  <Input
                    value={formData.description}
                    onChange={(e) =>
                      setFormData({ ...formData, description: e.target.value })
                    }
                    className="mt-1"
                  />
                ) : (
                  <p className="text-sm mt-1">{formData.description}</p>
                )}
              </div>
            </CardContent>
          </Card>

          {/* Tabs */}
          <Card>
            <Tabs defaultValue="overview" className="w-full">
              <div className="px-6 pt-6">
                <TabsList>
                  <TabsTrigger value="overview">Overview</TabsTrigger>
                  <TabsTrigger value="details">Details</TabsTrigger>
                  <TabsTrigger value="activity">Activity</TabsTrigger>
                  <TabsTrigger value="settings">Settings</TabsTrigger>
                </TabsList>
              </div>
              <CardContent className="pt-6">
                <TabsContent value="overview" className="space-y-4">
                  <div className="grid gap-4 md:grid-cols-2">
                    <div>
                      <Label className="text-muted-foreground">Field 1</Label>
                      <p className="text-sm font-medium">Value 1</p>
                    </div>
                    <div>
                      <Label className="text-muted-foreground">Field 2</Label>
                      <p className="text-sm font-medium">Value 2</p>
                    </div>
                    <div>
                      <Label className="text-muted-foreground">Field 3</Label>
                      <p className="text-sm font-medium">Value 3</p>
                    </div>
                    <div>
                      <Label className="text-muted-foreground">Field 4</Label>
                      <p className="text-sm font-medium">Value 4</p>
                    </div>
                  </div>
                </TabsContent>
                <TabsContent value="details" className="space-y-4">
                  <div className="space-y-4">
                    <div>
                      <Label>Additional Information</Label>
                      <p className="text-sm text-muted-foreground mt-1">
                        Detailed information about the entry goes here.
                      </p>
                    </div>
                    <Separator />
                    <div className="grid gap-4 md:grid-cols-2">
                      <div>
                        <Label className="text-muted-foreground">Metadata 1</Label>
                        <p className="text-sm font-medium">Data 1</p>
                      </div>
                      <div>
                        <Label className="text-muted-foreground">Metadata 2</Label>
                        <p className="text-sm font-medium">Data 2</p>
                      </div>
                    </div>
                  </div>
                </TabsContent>
                <TabsContent value="activity" className="space-y-4">
                  <Timeline items={timelineData} />
                </TabsContent>
                <TabsContent value="settings" className="space-y-4">
                  <div className="space-y-4">
                    <div>
                      <Label>Configuration Option 1</Label>
                      <Input className="mt-1" placeholder="Enter value" />
                    </div>
                    <div>
                      <Label>Configuration Option 2</Label>
                      <Input className="mt-1" placeholder="Enter value" />
                    </div>
                  </div>
                </TabsContent>
              </CardContent>
            </Tabs>
          </Card>
        </div>

        {/* Activity Timeline Sidebar */}
        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle>Recent Activity</CardTitle>
            </CardHeader>
            <CardContent>
              <Timeline items={timelineData} />
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  )
}
