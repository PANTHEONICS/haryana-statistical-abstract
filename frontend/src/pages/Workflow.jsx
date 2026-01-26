import { useState } from "react"
import PageHeader from "@/components/layout/PageHeader"
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card"
import { Stepper } from "@/components/ui/Stepper"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { ArrowLeft, ArrowRight, Check } from "lucide-react"

const steps = [
  {
    title: "Initialization",
    description: "Set up the initial parameters",
  },
  {
    title: "Processing",
    description: "Execute the main process",
  },
  {
    title: "Validation",
    description: "Verify the results",
  },
  {
    title: "Completion",
    description: "Finalize and confirm",
  },
]

const stepContent = {
  0: {
    title: "Initialization Step",
    description: "Configure the initial settings for the workflow.",
    fields: [
      { label: "Setting 1", value: "Value 1" },
      { label: "Setting 2", value: "Value 2" },
      { label: "Setting 3", value: "Value 3" },
    ],
  },
  1: {
    title: "Processing Step",
    description: "The main processing is currently running.",
    fields: [
      { label: "Process ID", value: "PROC-12345" },
      { label: "Status", value: "In Progress" },
      { label: "Progress", value: "65%" },
    ],
  },
  2: {
    title: "Validation Step",
    description: "Validating the processed results.",
    fields: [
      { label: "Validation Status", value: "Pending" },
      { label: "Checks Passed", value: "8/10" },
      { label: "Errors", value: "0" },
    ],
  },
  3: {
    title: "Completion Step",
    description: "Workflow has been completed successfully.",
    fields: [
      { label: "Final Status", value: "Completed" },
      { label: "Total Duration", value: "2h 34m" },
      { label: "Result", value: "Success" },
    ],
  },
}

export default function Workflow() {
  const [currentStep, setCurrentStep] = useState(1)
  const content = stepContent[currentStep]

  const handleNext = () => {
    if (currentStep < steps.length - 1) {
      setCurrentStep(currentStep + 1)
    }
  }

  const handlePrevious = () => {
    if (currentStep > 0) {
      setCurrentStep(currentStep - 1)
    }
  }

  return (
    <div className="space-y-6 p-6">
      <PageHeader
        title="Workflow Progress"
        breadcrumbs={["Home", "Workflow"]}
        description="Track and manage workflow progress"
      />

      <Card>
        <CardHeader>
          <CardTitle>Workflow Steps</CardTitle>
          <CardDescription>Current progress through the workflow</CardDescription>
        </CardHeader>
        <CardContent>
          <Stepper steps={steps} currentStep={currentStep} />
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>{content.title}</CardTitle>
          <CardDescription>{content.description}</CardDescription>
        </CardHeader>
        <CardContent className="space-y-6">
          <div className="grid gap-4 md:grid-cols-3">
            {content.fields.map((field, index) => (
              <div key={index} className="space-y-2">
                <p className="text-sm text-muted-foreground">{field.label}</p>
                <p className="text-base font-medium">{field.value}</p>
              </div>
            ))}
          </div>

          <Separator />

          <div className="flex items-center justify-between">
            <Button
              variant="outline"
              onClick={handlePrevious}
              disabled={currentStep === 0}
            >
              <ArrowLeft className="mr-2 h-4 w-4" />
              Previous
            </Button>
            <div className="flex gap-2">
              {currentStep < steps.length - 1 ? (
                <Button onClick={handleNext}>
                  Next
                  <ArrowRight className="ml-2 h-4 w-4" />
                </Button>
              ) : (
                <Button>
                  <Check className="mr-2 h-4 w-4" />
                  Complete
                </Button>
              )}
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
