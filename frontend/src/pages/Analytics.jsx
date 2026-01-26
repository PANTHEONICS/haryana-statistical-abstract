import { useState } from "react"
import PageHeader from "@/components/layout/PageHeader"
import { ChartCard } from "@/components/ui/ChartCard"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { DataTable } from "@/components/ui/DataTable"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  PieChart,
  Pie,
  Cell,
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts"
import { ArrowUp, ArrowDown, Download, Calendar } from "lucide-react"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"

const lineData = [
  { name: "Jan", value: 4000, value2: 2400 },
  { name: "Feb", value: 3000, value2: 1398 },
  { name: "Mar", value: 2000, value2: 9800 },
  { name: "Apr", value: 2780, value2: 3908 },
  { name: "May", value: 1890, value2: 4800 },
  { name: "Jun", value: 2390, value2: 3800 },
]

const pieData = [
  { name: "Category A", value: 400 },
  { name: "Category B", value: 300 },
  { name: "Category C", value: 200 },
  { name: "Category D", value: 100 },
]

const COLORS = [
  "hsl(var(--primary))",
  "hsl(var(--muted-foreground))",
  "hsl(var(--destructive))",
  "hsl(var(--accent))",
]

const metrics = [
  { label: "Total Views", value: "124,567", change: "+12.5%", trend: "up" },
  { label: "Conversion Rate", value: "3.24%", change: "+0.5%", trend: "up" },
  { label: "Avg. Duration", value: "4m 32s", change: "-5.2%", trend: "down" },
  { label: "Bounce Rate", value: "42.1%", change: "-2.3%", trend: "down" },
]

const tableData = [
  { id: 1, item: "Item A", views: 1234, conversions: 45, rate: "3.6%" },
  { id: 2, item: "Item B", views: 987, conversions: 32, rate: "3.2%" },
  { id: 3, item: "Item C", views: 765, conversions: 28, rate: "3.7%" },
  { id: 4, item: "Item D", views: 654, conversions: 21, rate: "3.2%" },
]

const tableColumns = [
  { key: "item", label: "Item", sortable: true },
  { key: "views", label: "Views", sortable: true },
  { key: "conversions", label: "Conversions", sortable: true },
  { key: "rate", label: "Rate", sortable: true },
]

export default function Analytics() {
  return (
    <div className="space-y-6 p-6">
      <PageHeader
        title="Analytics & Insights"
        breadcrumbs={["Home", "Analytics"]}
        description="Comprehensive analytics and performance metrics"
        primaryAction={{
          label: "Export",
          icon: Download,
          onClick: () => console.log("Export"),
        }}
        secondaryActions={[
          {
            label: "Date Range",
            icon: Calendar,
            variant: "outline",
            onClick: () => console.log("Date range"),
          },
        ]}
      />

      {/* Metrics Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {metrics.map((metric, index) => (
          <Card key={index}>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">{metric.label}</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{metric.value}</div>
              <div className="flex items-center text-xs text-muted-foreground mt-1">
                {metric.trend === "up" ? (
                  <ArrowUp className="h-3 w-3 text-green-500 mr-1" />
                ) : (
                  <ArrowDown className="h-3 w-3 text-red-500 mr-1" />
                )}
                <span
                  className={
                    metric.trend === "up" ? "text-green-500" : "text-red-500"
                  }
                >
                  {metric.change}
                </span>
                <span className="ml-1">vs last period</span>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Charts Grid */}
      <div className="grid gap-4 md:grid-cols-2">
        <ChartCard title="Trend Analysis" description="Line chart showing trends">
          <ResponsiveContainer width="100%" height={300}>
            <LineChart data={lineData}>
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

        <ChartCard title="Distribution" description="Pie chart breakdown">
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={pieData}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={({ name, percent }) =>
                  `${name} ${(percent * 100).toFixed(0)}%`
                }
                outerRadius={80}
                fill="#8884d8"
                dataKey="value"
              >
                {pieData.map((entry, index) => (
                  <Cell
                    key={`cell-${index}`}
                    fill={COLORS[index % COLORS.length]}
                  />
                ))}
              </Pie>
              <Tooltip />
            </PieChart>
          </ResponsiveContainer>
        </ChartCard>

        <ChartCard title="Performance Metrics" description="Bar chart comparison">
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={lineData}>
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

        <ChartCard title="Area Analysis" description="Area chart visualization">
          <ResponsiveContainer width="100%" height={300}>
            <AreaChart data={lineData}>
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
      </div>

      {/* Data Table */}
      <Card>
        <CardHeader>
          <CardTitle>Detailed Breakdown</CardTitle>
        </CardHeader>
        <CardContent>
          <DataTable columns={tableColumns} data={tableData} searchable={false} selectable={false} />
        </CardContent>
      </Card>
    </div>
  )
}
