export default function PaisController(view) {
    
    this.getTitulosDaTabela = () => {
        return [
            { titulo: 'Código', orderby: 'Codigo', className: 'codigo' },            
            { titulo: 'Nome', width: '100%', orderby: "Nome" },
        ];
    };

    this.getDadosDaTabela = (item) => {
        return [item.codigo, item.nome];
    };

    this.getObjetoDeDados = () => {        
    };

    return {
        getTitulosDaTabela,
        getDadosDaTabela,
        getObjetoDeDados,
    };
}
