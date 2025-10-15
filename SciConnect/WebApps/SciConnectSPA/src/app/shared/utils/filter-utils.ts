export class FilterUtils {
 
  static parseId(value: number | string | null | undefined): number | null {
    if (value === null || value === undefined) {
      return null;
    }
    return typeof value === 'string' ? parseInt(value, 10) : value;
  }
}
