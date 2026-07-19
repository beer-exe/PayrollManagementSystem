import React from 'react';
import { SortDirection } from '../../hooks/useDataTable';
import './DataTable.css';

interface SortableHeaderProps {
  label: string;
  sortKey: string;
  currentSortKey: string | null;
  currentSortDirection: SortDirection;
  onSort: (key: string) => void;
  style?: React.CSSProperties;
}

export const SortableHeader: React.FC<SortableHeaderProps> = ({
  label,
  sortKey,
  currentSortKey,
  currentSortDirection,
  onSort,
  style
}) => {
  const isActive = currentSortKey === sortKey;

  let justifyContent = 'flex-start';
  if (style?.textAlign === 'right') {
    justifyContent = 'flex-end';
  } else if (style?.textAlign === 'center') {
    justifyContent = 'center';
  }

  return (
    <th 
      onClick={() => onSort(sortKey)} 
      style={{ cursor: 'pointer', ...style }}
      className={`sortable-header ${isActive ? 'active' : ''}`}
    >
      <div className="sortable-header-content" style={{ justifyContent }}>
        <span>{label}</span>
        <span className="sort-icon-container">
          <svg 
            className={`sort-icon sort-icon-up ${isActive && currentSortDirection === 'asc' ? 'active' : ''}`} 
            xmlns="http://www.w3.org/2000/svg" 
            viewBox="0 0 20 20" 
            fill="currentColor"
          >
            <path fillRule="evenodd" d="M14.77 12.79a.75.75 0 01-1.06-.02L10 8.832 6.29 12.77a.75.75 0 11-1.08-1.04l4.25-4.5a.75.75 0 011.08 0l4.25 4.5a.75.75 0 01-.02 1.06z" clipRule="evenodd" />
          </svg>
          <svg 
            className={`sort-icon sort-icon-down ${isActive && currentSortDirection === 'desc' ? 'active' : ''}`} 
            xmlns="http://www.w3.org/2000/svg" 
            viewBox="0 0 20 20" 
            fill="currentColor"
          >
            <path fillRule="evenodd" d="M5.23 7.21a.75.75 0 011.06.02L10 11.168l3.71-3.938a.75.75 0 111.08 1.04l-4.25 4.5a.75.75 0 01-1.08 0l-4.25-4.5a.75.75 0 01.02-1.06z" clipRule="evenodd" />
          </svg>
        </span>
      </div>
    </th>
  );
};
