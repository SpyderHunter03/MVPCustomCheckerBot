import { useMemo } from 'react';
import NavigationBar from '../components/navigation/NavigationBar';
import TanstackTable from '../components/table/TanstackTable';
import { ColumnDef, Cell } from '@tanstack/react-table';  // Import the type
import Spinner from '../components/loading/Spinner';
import { useQuery } from '@tanstack/react-query';

interface AvailableMolds {
    id: number;
    plastic: string;
    mold: string;
    weight: string;
    dateAvailable: string;
}

type TableMold = {
  id: number;
  plastic: string;
  mold: string;
  weight: string;
  dateAvailable: Date; // Assuming you'll convert string to Date immediately after fetching
}

function AvailableMoldsPage() {
  const { isPending, data } = useQuery({
    queryKey: ['availablemolds'], queryFn: async ({ queryKey }) => {
      try {
        const response = await fetch(`/api/${queryKey[0]}`);
        if (!response.ok) throw new Error('Failed to fetch molds');

        const data: AvailableMolds[] = await response.json();

        return data.map(mold => ({
          ...mold,
          dateAvailable: new Date(mold.dateAvailable)
        }));
      } catch (error) {
        console.error(error);
        throw error; // Re-throw the error to be caught by calling function
      }
    }
  })

  const columns: ColumnDef<TableMold>[] = useMemo(() => [
    { accessorKey: 'plastic', header: 'Plastic', enableSorting: true, enableFiltering: true, },
    { accessorKey: 'mold', header: 'Mold', enableSorting: true, enableFiltering: true, },
    { accessorKey: 'weight', header: 'Weights', enableSorting: true, enableFiltering: true, },
    {
      accessorKey: 'dateAvailable',
      header: 'Date Available',
      cell: (info: Cell<TableMold, Date>) => info.getValue().toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: '2-digit'
      }),
      enableSorting: true,
      enableFiltering: true,
    },
  ], []);

  return (
    <div className="flex flex-col min-h-screen">
      <NavigationBar />
      <div className="pt-16 px-16 text-center">
        <span className="text-xl uppercase font-bold">Available Molds</span>
        { isPending && (
          <Spinner />
        )}
        <TanstackTable<TableMold> columns={columns} data={data || []} />
      </div>
    </div>
  );
}

export default AvailableMoldsPage;