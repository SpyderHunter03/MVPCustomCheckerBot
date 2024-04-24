import { useEffect, useState } from 'react';
import {
  useReactTable,
  getCoreRowModel,
  ColumnDef,
  flexRender,
} from '@tanstack/react-table';

interface TableProps<T extends object> {
  columns?: ColumnDef<T>[];
  data: T[];
  className?: string;
}

function TanstackTable<T extends object>({ columns: initialColumns, data, className }: TableProps<T>) {
  const [columns, setColumns] = useState<ColumnDef<T>[]>(initialColumns || []);

  useEffect(() => {
    if (!initialColumns && data[0]) {
      // Automatically generate columns from data keys if not provided
      const autoColumns: ColumnDef<T>[] = Object.keys(data[0]).map(key => ({
        accessorKey: key as keyof T,
        header: key.toUpperCase(),
        cell: info => info.getValue() as string,
      }));
      setColumns(autoColumns);
    }
  }, [data, initialColumns]);

  const tableInstance = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
  });

  return (
    <table className={`${className} w-full border-collapse`}>
      <thead className="bg-gray-100">
        {tableInstance.getHeaderGroups().map(headerGroup => (
          <tr className="border-b" key={headerGroup.id}>
            {headerGroup.headers.map(header => (
              <th className="border p-2" key={header.id}>
                {header.isPlaceholder
                  ? null
                  : flexRender(header.column.columnDef.header, header.getContext())}
              </th>
            ))}
          </tr>
        ))}
      </thead>
      <tbody>
        {tableInstance.getRowModel().rows.map(row => (
          <tr className="border-b" key={row.id}>
            {row.getVisibleCells().map(cell => (
              <td className="border p-2" key={cell.id}>
                {flexRender(cell.column.columnDef.cell, cell.getContext())}
              </td>
            ))}
          </tr>
        ))}
      </tbody>
    </table>
  );
}

export default TanstackTable;