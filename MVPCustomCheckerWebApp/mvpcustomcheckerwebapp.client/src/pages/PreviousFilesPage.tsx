import { useMemo } from 'react';
import NavigationBar from '../components/navigation/NavigationBar';
import TanstackTable from '../components/table/TanstackTable';
import { ColumnDef, Cell } from '@tanstack/react-table';  // Import the type
import Spinner from '../components/loading/Spinner';
import { useQuery } from '@tanstack/react-query';

interface File {
  id: number;
  fileName: string;
  fileLocation: string;
}

function PreviousFilesPage() {
  const { isPending, data } = useQuery({
    queryKey: ['filelocations'], queryFn: async ({ queryKey }) => {
      try {
        const response = await fetch(`/api/${queryKey[0]}`);
        if (!response.ok) throw new Error('Failed to fetch molds');

        const data: File[] = await response.json();

        return data;
      } catch (error) {
        console.error(error);
        throw error; // Re-throw the error to be caught by calling function
      }
    }
  })

  const columns: ColumnDef<File>[] = useMemo(() => [
    { accessorKey: 'fileName', header: 'Name', enableSorting: true, enableFiltering: true, },
    {
      accessorKey: 'fileLocation',
      header: 'Download File',
      cell: (info: Cell<File, string>) => {
        console.log(info);
        return (
          <a href={`/api/download/${info.row.original.id}`} download>Download</a>
        );
      }
    },
  ], []);

  return (
    <div className="flex flex-col min-h-screen">
      <NavigationBar />
      <div className="pt-16 px-16 text-center">
        <span className="text-xl uppercase font-bold">Previous Files</span>
        {isPending && (
          <Spinner />
        )}
        <TanstackTable<File> columns={columns} data={data || []} />
      </div>
    </div>
  );
}

export default PreviousFilesPage;