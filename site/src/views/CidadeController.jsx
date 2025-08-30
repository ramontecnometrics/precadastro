export default function CidadeController () {

    const getTitulosDaTabela = () => {
        return [
            { titulo: 'Código', orderby: 'Id', className: 'codigo' },            
            { titulo: 'Cidade', width: '60%', orderby: null },
            { titulo: 'Estado', width: '30%', orderby: null },
            { titulo: 'UF', width: '10%', orderby: null },
        ];
    };

    const getDadosDaTabela = (item) => {
        return [item.id, item.nome, item.estado.nome, item.estado.uf];
    };

    const getObjetoDeDados = () => {        
    };

    return {
        getTitulosDaTabela,
        getDadosDaTabela,
        getObjetoDeDados,
    };
}
