import * as React from "react"
import { cn } from "@/lib/utils"

const DrawerContext = React.createContext({
  open: false,
  setOpen: () => {},
})

const Drawer = ({ open, onOpenChange, children }) => {
  return (
    <DrawerContext.Provider value={{ open, setOpen: onOpenChange }}>
      {children}
    </DrawerContext.Provider>
  )
}

const DrawerTrigger = ({ className, children, ...props }) => {
  const { setOpen } = React.useContext(DrawerContext)
  return (
    <div
      className={cn("cursor-pointer", className)}
      onClick={() => setOpen(true)}
      {...props}
    >
      {children}
    </div>
  )
}

const DrawerOverlay = ({ className, ...props }) => {
  const { open, setOpen } = React.useContext(DrawerContext)
  if (!open) return null
  return (
    <div
      className={cn(
        "fixed inset-0 z-50 bg-background/80 backdrop-blur-sm",
        "data-[state=open]:animate-in data-[state=closed]:animate-out",
        "data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0"
      )}
      onClick={() => setOpen(false)}
      {...props}
    />
  )
}

const DrawerContent = ({ className, children, ...props }) => {
  const { open } = React.useContext(DrawerContext)
  if (!open) return null
  return (
    <>
      <DrawerOverlay />
      <div
        className={cn(
          "fixed inset-y-0 right-0 z-50 h-full w-full border-l bg-background shadow-lg transition-transform",
          "sm:max-w-sm",
          className
        )}
        {...props}
      >
        {children}
      </div>
    </>
  )
}

const DrawerHeader = ({ className, ...props }) => (
  <div
    className={cn("flex flex-col space-y-1.5 p-6", className)}
    {...props}
  />
)

const DrawerTitle = ({ className, ...props }) => (
  <h2
    className={cn("text-lg font-semibold leading-none tracking-tight", className)}
    {...props}
  />
)

const DrawerDescription = ({ className, ...props }) => (
  <p
    className={cn("text-sm text-muted-foreground", className)}
    {...props}
  />
)

const DrawerFooter = ({ className, ...props }) => (
  <div
    className={cn("flex flex-col-reverse sm:flex-row sm:justify-end sm:space-x-2 p-6 pt-0", className)}
    {...props}
  />
)

const DrawerClose = ({ className, children, ...props }) => {
  const { setOpen } = React.useContext(DrawerContext)
  return (
    <div
      className={cn("cursor-pointer", className)}
      onClick={() => setOpen(false)}
      {...props}
    >
      {children}
    </div>
  )
}

export {
  Drawer,
  DrawerTrigger,
  DrawerContent,
  DrawerHeader,
  DrawerTitle,
  DrawerDescription,
  DrawerFooter,
  DrawerClose,
}
