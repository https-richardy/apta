(function () {
    var tips = [
        "Preparando tudo para você...",
        "Carregando os dados mais recentes...",
        "Quase lá, só mais um instante...",
        "Estamos deixando tudo pronto...",
        "Sincronizando os dados da plataforma...",
        "Preparando o ambiente de trabalho...",
        "Estamos organizando tudo por aqui...",
        "Só mais alguns segundos...",
        "Conferindo as informações disponíveis...",
        "Carregando os detalhes da sua operação...",
        "Estamos cuidando dos últimos detalhes...",
        "Carregando os recursos da plataforma...",

        "Nada de apertar F5, estamos quase lá...",
        "Não é enrolação, realmente estamos carregando algumas informações...",
        "Até parece que demoramos de propósito para carregar tudo...",
        "Que tal aproveitar para pegar um cafezinho?"
    ];

    var element = document.getElementById("loading-tip");
    if (!element)
        return;

    function pick(previous) {
        let index = Math.floor(Math.random() * tips.length);
        while (tips.length > 1 && index === previous) {
            index = Math.floor(Math.random() * tips.length);
        }

        return index;
    }

    var current = pick(-1);

    element.textContent = tips[current];

    setInterval(function () {
        current = pick(current);
        element.textContent = tips[current];
    }, 3500);
})();
