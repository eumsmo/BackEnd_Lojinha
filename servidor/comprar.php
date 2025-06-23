<?php
	$nome = isset($_GET['nome']) ? $_GET['nome'] : NULL;
	$xp = isset($_GET['xp']) ? $_GET['xp'] : 0;
	$nivel = isset($_GET['nivel']) ? $_GET['nivel'] : 0;

	if ($nome == NULL || $nome == '') {
		header('Content-Type: application/json');
		echo json_encode([
			'status' => 'error',
			'mensagem' => 'Nenhum nome passado pelo parâmetro GET "nome".'
		]);
		exit;
	}


	$json_content = file_get_contents('./itens.json');
	$itens = json_decode($json_content, true);
	$itemAchado = NULL;

	foreach ($itens['itens'] as $i => $item) {
		if ($item['nome'] == $nome) {
			$itemAchado = $item;
			break;
		}
	}

	if ($itemAchado == NULL) {
		header('Content-Type: application/json');
		echo json_encode([
			'status' => 'error',
			'mensagem' => 'Nenhum item encontrado com o nome "' . $nome . '".'
		]);
		exit;
	}

	$preco = $itemAchado['preco'] * (1 + $nivel);
	if ($xp < $preco) {
		header('Content-Type: application/json');
		echo json_encode([
			'status' => 'error',
			'mensagem' => 'XP insuficiente para comprar o item "' . $nome . '".'
		]);
		exit;
	}

	$xpRestante = $xp - $preco;

	header('Content-Type: application/json');
	echo json_encode([
		'status' => 'success',
		'xpRestante' => $xpRestante,
		'item' => $itemAchado
	]);
	exit;
?>