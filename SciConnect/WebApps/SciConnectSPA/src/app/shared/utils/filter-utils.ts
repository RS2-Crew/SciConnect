/**
 * Utility functions for filtering and data manipulation in the Dashboard
 */
export class FilterUtils {
  /**
   * Safely parses a value that could be a string or number into a number.
   * Handles null values and string-to-number conversion.
   * 
   * @param value - The value to parse (can be number, string, or null)
   * @returns Parsed number or null
   */
  static parseId(value: number | string | null | undefined): number | null {
    if (value === null || value === undefined) {
      return null;
    }
    return typeof value === 'string' ? parseInt(value, 10) : value;
  }
}
