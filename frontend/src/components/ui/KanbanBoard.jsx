import { useState } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "./card"
import { Button } from "./button"
import { Plus, MoreVertical } from "lucide-react"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "./dropdown-menu"
import { cn } from "@/lib/utils"

export function KanbanBoard({ columns = [], onCardMove, onCardAdd }) {
  const [draggedCard, setDraggedCard] = useState(null)
  const [draggedColumn, setDraggedColumn] = useState(null)

  const handleDragStart = (card, columnId) => {
    setDraggedCard(card)
    setDraggedColumn(columnId)
  }

  const handleDragOver = (e) => {
    e.preventDefault()
  }

  const handleDrop = (targetColumnId) => {
    if (draggedCard && draggedColumn && targetColumnId !== draggedColumn) {
      onCardMove?.(draggedCard, draggedColumn, targetColumnId)
    }
    setDraggedCard(null)
    setDraggedColumn(null)
  }

  return (
    <div className="flex gap-4 overflow-x-auto pb-4">
      {columns.map((column) => (
        <div
          key={column.id}
          className="flex-shrink-0 w-80"
          onDragOver={handleDragOver}
          onDrop={() => handleDrop(column.id)}
        >
          <Card className="h-full flex flex-col">
            <CardHeader className="pb-3">
              <div className="flex items-center justify-between">
                <div>
                  <CardTitle className="text-base">{column.title}</CardTitle>
                  <span className="text-xs text-muted-foreground">
                    {column.cards?.length || 0} items
                  </span>
                </div>
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button variant="ghost" size="icon" className="h-8 w-8">
                      <MoreVertical className="h-4 w-4" />
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end">
                    <DropdownMenuItem>Edit Column</DropdownMenuItem>
                    <DropdownMenuItem className="text-destructive">
                      Delete Column
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </div>
            </CardHeader>
            <CardContent className="flex-1 overflow-y-auto space-y-2">
              {column.cards?.map((card) => (
                <div
                  key={card.id}
                  draggable
                  onDragStart={() => handleDragStart(card, column.id)}
                  className={cn(
                    "rounded-lg border bg-card p-4 shadow-sm transition-shadow",
                    "hover:shadow-md cursor-move"
                  )}
                >
                  <div className="flex items-start justify-between mb-2">
                    <h4 className="text-sm font-medium">{card.title}</h4>
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button variant="ghost" size="icon" className="h-6 w-6">
                          <MoreVertical className="h-3 w-3" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem>Edit</DropdownMenuItem>
                        <DropdownMenuItem>Duplicate</DropdownMenuItem>
                        <DropdownMenuItem className="text-destructive">
                          Delete
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </div>
                  {card.description && (
                    <p className="text-xs text-muted-foreground mb-2">
                      {card.description}
                    </p>
                  )}
                  {card.meta && (
                    <div className="flex items-center gap-2 mt-2">
                      {card.meta}
                    </div>
                  )}
                </div>
              ))}
              <Button
                variant="ghost"
                className="w-full justify-start text-muted-foreground"
                onClick={() => onCardAdd?.(column.id)}
              >
                <Plus className="mr-2 h-4 w-4" />
                Add Card
              </Button>
            </CardContent>
          </Card>
        </div>
      ))}
    </div>
  )
}
