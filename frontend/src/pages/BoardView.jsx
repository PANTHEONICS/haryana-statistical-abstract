import { useState } from "react"
import PageHeader from "@/components/layout/PageHeader"
import { KanbanBoard } from "@/components/ui/KanbanBoard"
import { StatusBadge } from "@/components/ui/StatusBadge"
import { Plus } from "lucide-react"

const initialColumns = [
  {
    id: "todo",
    title: "To Do",
    cards: [
      {
        id: "1",
        title: "Task A",
        description: "Complete the initial setup",
        meta: <StatusBadge status="pending" />,
      },
      {
        id: "2",
        title: "Task B",
        description: "Review documentation",
        meta: <StatusBadge status="pending" />,
      },
    ],
  },
  {
    id: "in-progress",
    title: "In Progress",
    cards: [
      {
        id: "3",
        title: "Task C",
        description: "Implement feature X",
        meta: <StatusBadge status="active" />,
      },
      {
        id: "4",
        title: "Task D",
        description: "Fix bug in module Y",
        meta: <StatusBadge status="active" />,
      },
    ],
  },
  {
    id: "review",
    title: "Review",
    cards: [
      {
        id: "5",
        title: "Task E",
        description: "Code review for PR #123",
        meta: <StatusBadge status="info" />,
      },
    ],
  },
  {
    id: "done",
    title: "Done",
    cards: [
      {
        id: "6",
        title: "Task F",
        description: "Completed feature Z",
        meta: <StatusBadge status="success" />,
      },
      {
        id: "7",
        title: "Task G",
        description: "Documentation updated",
        meta: <StatusBadge status="success" />,
      },
    ],
  },
]

export default function BoardView() {
  const [columns, setColumns] = useState(initialColumns)

  const handleCardMove = (card, fromColumnId, toColumnId) => {
    setColumns((prev) => {
      const newColumns = prev.map((col) => ({ ...col, cards: [...col.cards] }))
      const fromCol = newColumns.find((c) => c.id === fromColumnId)
      const toCol = newColumns.find((c) => c.id === toColumnId)

      const cardIndex = fromCol.cards.findIndex((c) => c.id === card.id)
      if (cardIndex !== -1) {
        const [movedCard] = fromCol.cards.splice(cardIndex, 1)
        toCol.cards.push(movedCard)
      }

      return newColumns
    })
  }

  const handleCardAdd = (columnId) => {
    const newCard = {
      id: Date.now().toString(),
      title: `New Task`,
      description: "Add description here",
      meta: <StatusBadge status="pending" />,
    }

    setColumns((prev) =>
      prev.map((col) =>
        col.id === columnId
          ? { ...col, cards: [...col.cards, newCard] }
          : col
      )
    )
  }

  return (
    <div className="space-y-6 p-6">
      <PageHeader
        title="Board View"
        breadcrumbs={["Home", "Board View"]}
        description="Manage items in a kanban board layout"
        primaryAction={{
          label: "Add Column",
          icon: Plus,
          onClick: () => console.log("Add column"),
        }}
      />

      <KanbanBoard
        columns={columns}
        onCardMove={handleCardMove}
        onCardAdd={handleCardAdd}
      />
    </div>
  )
}
