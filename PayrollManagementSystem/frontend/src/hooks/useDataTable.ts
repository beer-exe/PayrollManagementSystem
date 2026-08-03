import { useState, useMemo } from 'react';

export type SortDirection = 'asc' | 'desc' | null;

interface UseDataTableProps<T> {
  data: T[];
  initialPageSize?: number;
  searchableFields?: (keyof T | string)[];
  customFilter?: (item: T, searchTerm: string) => boolean;
}

export function useDataTable<T>({ 
  data, 
  initialPageSize = 10,
  searchableFields = [],
  customFilter
}: UseDataTableProps<T>) {
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(initialPageSize);
  const [sortKey, setSortKey] = useState<string | null>(null);
  const [sortDirection, setSortDirection] = useState<SortDirection>(null);
  const [searchTerm, setSearchTerm] = useState('');

  // 1. Filter
  const filteredData = useMemo(() => {
    let result = [...data];
    if (searchTerm) {
      const lowerTerm = searchTerm.toLowerCase();
      result = result.filter(item => {
        if (customFilter) {
          return customFilter(item, lowerTerm);
        }
        
        for (const field of searchableFields) {
          const value = (item as any)[field];
          if (value != null && String(value).toLowerCase().includes(lowerTerm)) {
            return true;
          }
        }
        return false;
      });
    }
    return result;
  }, [data, searchTerm, searchableFields, customFilter]);

  // 2. Sort
  const sortedData = useMemo(() => {
    if (!sortKey || !sortDirection) return filteredData;

    return [...filteredData].sort((a: any, b: any) => {
      const aValue = a[sortKey];
      const bValue = b[sortKey];

      if (aValue === bValue) return 0;
      
      const comparison = aValue > bValue ? 1 : -1;
      return sortDirection === 'asc' ? comparison : -comparison;
    });
  }, [filteredData, sortKey, sortDirection]);

  // 3. Paginate
  const totalItems = sortedData.length;
  const totalPages = Math.max(1, Math.ceil(totalItems / pageSize));
  
  const validCurrentPage = Math.min(currentPage, totalPages);

  const paginatedData = useMemo(() => {
    const startIndex = (validCurrentPage - 1) * pageSize;
    return sortedData.slice(startIndex, startIndex + pageSize);
  }, [sortedData, validCurrentPage, pageSize]);

  const handleSort = (key: string) => {
    if (sortKey === key) {
      if (sortDirection === 'asc') setSortDirection('desc');
      else if (sortDirection === 'desc') {
        setSortDirection(null);
        setSortKey(null);
      }
    } else {
      setSortKey(key);
      setSortDirection('asc');
    }
    setCurrentPage(1);
  };

  const handleSearch = (term: string) => {
    setSearchTerm(term);
    setCurrentPage(1);
  };

  return {
    currentData: paginatedData,
    allFilteredAndSortedData: sortedData,
    
    currentPage: validCurrentPage,
    pageSize,
    totalItems,
    totalPages,
    setCurrentPage,
    setPageSize,
    
    sortKey,
    sortDirection,
    handleSort,
    
    searchTerm,
    setSearchTerm: handleSearch
  };
}
