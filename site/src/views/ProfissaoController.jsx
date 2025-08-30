export default function ProfissaoController () {

    const getTitulosDaTabela = () => {
        return [
            { titulo: 'Código', orderby: 'Id', className: 'codigo' },            
            { titulo: 'Descrição', width: '100%', orderby: 'nome' }, 
        ];
    };

    const getDadosDaTabela = (item) => {
        return [item.id, item.nome];
    };

    const getObjetoDeDados = () => {        
    };

    return {
        getTitulosDaTabela,
        getDadosDaTabela,
        getObjetoDeDados,
    };
}
