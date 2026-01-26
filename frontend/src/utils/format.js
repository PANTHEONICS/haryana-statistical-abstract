/**
 * Formatting Utilities
 * Provides consistent formatting functions across the application
 */

/**
 * Format a number with thousand separators
 * @param {number|null|undefined} num - Number to format
 * @param {object} options - Formatting options
 * @returns {string} Formatted number or '-'
 * 
 * @example
 * formatNumber(1234567) // "1,234,567"
 * formatNumber(1234567.89) // "1,234,567.89"
 * formatNumber(null) // "-"
 */
export function formatNumber(num, options = {}) {
  if (num === null || num === undefined || isNaN(num)) {
    return options.emptyValue || '-';
  }

  const {
    decimals = null, // Number of decimal places (null = auto)
    locale = 'en-IN', // Locale for formatting
    minimumFractionDigits = 0,
    maximumFractionDigits = decimals !== null ? decimals : 20,
  } = options;

  try {
    return new Intl.NumberFormat(locale, {
      minimumFractionDigits,
      maximumFractionDigits,
    }).format(num);
  } catch (error) {
    // Fallback to simple formatting
    if (decimals !== null) {
      return num.toFixed(decimals).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    }
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
  }
}

/**
 * Format a number as percentage
 * @param {number|null|undefined} num - Number to format (0-100 or 0-1)
 * @param {object} options - Formatting options
 * @returns {string} Formatted percentage or '-'
 * 
 * @example
 * formatPercentage(12.34) // "12.34%"
 * formatPercentage(0.1234, { asDecimal: true }) // "12.34%"
 * formatPercentage(null) // "-"
 */
export function formatPercentage(num, options = {}) {
  if (num === null || num === undefined || isNaN(num)) {
    return options.emptyValue || '-';
  }

  const {
    decimals = 2,
    asDecimal = false, // If true, treat num as decimal (0.1234 = 12.34%)
    emptyValue = '-',
  } = options;

  const percentage = asDecimal ? num * 100 : num;
  return `${percentage.toFixed(decimals)}%`;
}

/**
 * Format a date string
 * @param {string|Date|null|undefined} date - Date to format
 * @param {object} options - Formatting options
 * @returns {string} Formatted date or '-'
 * 
 * @example
 * formatDate('2024-01-15') // "Jan 15, 2024"
 * formatDate('2024-01-15', { format: 'short' }) // "1/15/24"
 * formatDate('2024-01-15', { format: 'long' }) // "January 15, 2024"
 */
export function formatDate(date, options = {}) {
  if (!date) {
    return options.emptyValue || '-';
  }

  const {
    format = 'medium', // 'short' | 'medium' | 'long' | 'full' | 'custom'
    customFormat = null, // Custom format string (if format === 'custom')
    locale = 'en-US',
    emptyValue = '-',
  } = options;

  try {
    const dateObj = date instanceof Date ? date : new Date(date);
    
    if (isNaN(dateObj.getTime())) {
      return emptyValue;
    }

    if (format === 'custom' && customFormat) {
      // Simple custom format implementation
      return customFormat
        .replace('YYYY', dateObj.getFullYear())
        .replace('MM', String(dateObj.getMonth() + 1).padStart(2, '0'))
        .replace('DD', String(dateObj.getDate()).padStart(2, '0'))
        .replace('HH', String(dateObj.getHours()).padStart(2, '0'))
        .replace('mm', String(dateObj.getMinutes()).padStart(2, '0'))
        .replace('ss', String(dateObj.getSeconds()).padStart(2, '0'));
    }

    const formatOptions = {
      short: { year: 'numeric', month: 'numeric', day: 'numeric' },
      medium: { year: 'numeric', month: 'short', day: 'numeric' },
      long: { year: 'numeric', month: 'long', day: 'numeric' },
      full: { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' },
    };

    return new Intl.DateTimeFormat(locale, formatOptions[format] || formatOptions.medium).format(dateObj);
  } catch (error) {
    console.error('Failed to format date:', error);
    return emptyValue;
  }
}

/**
 * Format currency
 * @param {number|null|undefined} amount - Amount to format
 * @param {object} options - Formatting options
 * @returns {string} Formatted currency or '-'
 * 
 * @example
 * formatCurrency(1234.56) // "$1,234.56"
 * formatCurrency(1234.56, { currency: 'INR' }) // "₹1,234.56"
 */
export function formatCurrency(amount, options = {}) {
  if (amount === null || amount === undefined || isNaN(amount)) {
    return options.emptyValue || '-';
  }

  const {
    currency = 'USD',
    locale = 'en-US',
    minimumFractionDigits = 2,
    maximumFractionDigits = 2,
    emptyValue = '-',
  } = options;

  try {
    return new Intl.NumberFormat(locale, {
      style: 'currency',
      currency,
      minimumFractionDigits,
      maximumFractionDigits,
    }).format(amount);
  } catch (error) {
    console.error('Failed to format currency:', error);
    return emptyValue;
  }
}

/**
 * Format file size
 * @param {number} bytes - Size in bytes
 * @param {object} options - Formatting options
 * @returns {string} Formatted file size
 * 
 * @example
 * formatFileSize(1024) // "1 KB"
 * formatFileSize(1048576) // "1 MB"
 */
export function formatFileSize(bytes, options = {}) {
  if (!bytes || bytes === 0) {
    return '0 Bytes';
  }

  const { decimals = 2 } = options;
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));

  return `${parseFloat((bytes / Math.pow(k, i)).toFixed(decimals))} ${sizes[i]}`;
}

export default {
  formatNumber,
  formatPercentage,
  formatDate,
  formatCurrency,
  formatFileSize,
};
