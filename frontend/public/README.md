# Public Assets Directory

This directory contains static assets that are served directly by the application.

## Haryana Government Emblem Placement

To add the official Haryana Government Emblem:

1. Place your emblem image file in this directory (`frontend/public/`)
2. Supported formats: `.png`, `.jpg`, `.jpeg`, `.svg`
3. **Recommended filename: `haryana-emblem.png`**
4. Recommended size: 300x300px or larger (will be scaled down to 80-112px in header)
5. The emblem should be the official Haryana Government logo with:
   - Lion Capital of Ashoka at top
   - State emblem with lotus and sun
   - "हरियाणा सरकार" / "GOVT. OF HARYANA" text

## Current Emblem Path

The header component looks for: `/haryana-emblem.png`

If you use a different filename, update the `logoPath` variable in:
`frontend/src/components/layout/DepartmentHeader.jsx`

## Example

```
frontend/public/haryana-emblem.png  → Accessible as /haryana-emblem.png
frontend/public/haryana-emblem.jpg  → Accessible as /haryana-emblem.jpg
frontend/public/haryana-emblem.svg  → Accessible as /haryana-emblem.svg
```
