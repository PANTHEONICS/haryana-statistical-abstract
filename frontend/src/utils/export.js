/**
 * Export data to CSV file
 * 
 * @param {Array} data - Data array to export
 * @param {Array} columns - Column definitions [{ key, label }]
 * @param {string} filename - Filename (without extension)
 * @returns {void}
 * 
 * @example
 * exportToCSV(data, [
 *   { key: 'year', label: 'Year' },
 *   { key: 'population', label: 'Population' }
 * ], 'census_data')
 */
export function exportToCSV(data, columns, filename = 'export') {
  if (!data || !Array.isArray(data) || data.length === 0) {
    alert('No data to export');
    return;
  }

  if (!columns || !Array.isArray(columns) || columns.length === 0) {
    alert('No columns defined for export');
    return;
  }

  try {
    // Create header row
    const headerRow = columns.map(col => col.label || col.key).join(',');

    // Create data rows
    const dataRows = data.map(record => {
      return columns.map(col => {
        const value = record[col.key];
        
        // Handle null/undefined
        if (value === null || value === undefined) {
          return '';
        }
        
        // Handle arrays/objects - convert to JSON string
        if (typeof value === 'object') {
          return JSON.stringify(value);
        }
        
        // Handle strings with commas - wrap in quotes
        if (typeof value === 'string' && value.includes(',')) {
          return `"${value.replace(/"/g, '""')}"`; // Escape quotes
        }
        
        return value;
      }).join(',');
    });

    // Combine header and data
    const csvContent = [headerRow, ...dataRows].join('\n');

    // Create blob and download
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    
    // Generate filename with date
    const dateStr = new Date().toISOString().split('T')[0];
    link.download = `${filename}_${dateStr}.csv`;
    
    // Trigger download
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    // Clean up
    window.URL.revokeObjectURL(url);
  } catch (error) {
    console.error('Failed to export CSV:', error);
    alert('Failed to export data. Please try again.');
  }
}

export default exportToCSV;
