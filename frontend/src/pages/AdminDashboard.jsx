import { useState } from "react"
import PageHeader from "@/components/layout/PageHeader"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { ChartCard } from "@/components/ui/ChartCard"
import { Timeline } from "@/components/ui/Timeline"
import { Button } from "@/components/ui/button"
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts"
import { ArrowUp, ArrowDown, TrendingUp, Download, Filter } from "lucide-react"

const kpiData = [
  { label: "Total Items", value: "12,345", change: "+12.5%", trend: "up" },
  { label: "Active Users", value: "8,234", change: "+5.2%", trend: "up" },
  { label: "Revenue", value: "$124,567", change: "-2.1%", trend: "down" },
  { label: "Completion Rate", value: "87.3%", change: "+3.4%", trend: "up" },
]

const chartData = [
  { name: "Jan", value: 4000, value2: 2400 },
  { name: "Feb", value: 3000, value2: 1398 },
  { name: "Mar", value: 2000, value2: 9800 },
  { name: "Apr", value: 2780, value2: 3908 },
  { name: "May", value: 1890, value2: 4800 },
  { name: "Jun", value: 2390, value2: 3800 },
]

const timelineData = [
  {
    title: "System Update Completed",
    description: "All systems are now running on the latest version",
    time: "2 hours ago",
    status: "completed",
  },
  {
    title: "New Entry Created",
    description: "A new entry was added to the system",
    time: "4 hours ago",
    status: "completed",
  },
  {
    title: "Processing Request",
    description: "Currently processing batch operation",
    time: "6 hours ago",
    status: "active",
  },
  {
    title: "Scheduled Maintenance",
    description: "Maintenance window scheduled for next week",
    time: "1 day ago",
    status: "pending",
  },
]

const quickActions = [
  { label: "Create New", icon: "➕" },
  { label: "Import Data", icon: "📥" },
  { label: "Generate Report", icon: "📊" },
  { label: "Settings", icon: "⚙️" },
]

export default function AdminDashboard() {
  return (
    <div className="space-y-6 p-6">
      <PageHeader
        title="Admin Dashboard"
        breadcrumbs={["Home", "Dashboard"]}
        primaryAction={{
          label: "Export Report",
          icon: Download,
          onClick: () => console.log("Export"),
        }}
        secondaryActions={[
          {
            label: "Filter",
            icon: Filter,
            variant: "outline",
            onClick: () => console.log("Filter"),
          },
        ]}
      />

      {/* KPI Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {kpiData.map((kpi, index) => (
          <Card key={index}>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">{kpi.label}</CardTitle>
              <TrendingUp className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{kpi.value}</div>
              <div className="flex items-center text-xs text-muted-foreground mt-1">
                {kpi.trend === "up" ? (
                  <ArrowUp className="h-3 w-3 text-green-500 mr-1" />
                ) : (
                  <ArrowDown className="h-3 w-3 text-red-500 mr-1" />
                )}
                <span
                  className={
                    kpi.trend === "up" ? "text-green-500" : "text-red-500"
                  }
                >
                  {kpi.change}
                </span>
                <span className="ml-1">from last month</span>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Charts Section */}
      <div className="grid gap-4 md:grid-cols-2">
        <ChartCard title="Performance Overview" description="Last 6 months">
          <ResponsiveContainer width="100%" height={300}>
            <LineChart data={chartData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis />
              <Tooltip />
              <Legend />
              <Line
                type="monotone"
                dataKey="value"
                stroke="hsl(var(--primary))"
                strokeWidth={2}
              />
              <Line
                type="monotone"
                dataKey="value2"
                stroke="hsl(var(--muted-foreground))"
                strokeWidth={2}
              />
            </LineChart>
          </ResponsiveContainer>
        </ChartCard>

        <ChartCard title="Activity Distribution" description="Monthly breakdown">
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={chartData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis />
              <Tooltip />
              <Legend />
              <Bar dataKey="value" fill="hsl(var(--primary))" />
              <Bar dataKey="value2" fill="hsl(var(--muted-foreground))" />
            </BarChart>
          </ResponsiveContainer>
        </ChartCard>
      </div>

      <div className="grid gap-4 md:grid-cols-2">
        <ChartCard title="Trend Analysis" description="Area chart visualization">
          <ResponsiveContainer width="100%" height={300}>
            <AreaChart data={chartData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis />
              <Tooltip />
              <Area
                type="monotone"
                dataKey="value"
                stroke="hsl(var(--primary))"
                fill="hsl(var(--primary))"
                fillOpacity={0.2}
              />
            </AreaChart>
          </ResponsiveContainer>
        </ChartCard>

        <Card>
          <CardHeader>
            <CardTitle>Recent Activity</CardTitle>
          </CardHeader>
          <CardContent>
            <Timeline items={timelineData} />
          </CardContent>
        </Card>
      </div>

      {/* Quick Actions */}
      <Card>
        <CardHeader>
          <CardTitle>Quick Actions</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid gap-4 md:grid-cols-4">
            {quickActions.map((action, index) => (
              <Button
                key={index}
                variant="outline"
                className="h-24 flex-col gap-2"
              >
                <span className="text-2xl">{action.icon}</span>
                <span>{action.label}</span>
              </Button>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
