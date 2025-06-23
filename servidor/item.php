<?php
	$nome = isset($_GET['nome']) ? $_GET['nome'] : NULL;

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

	header('Content-Type: application/json');
	echo json_encode([
		'status' => 'success',
		'item' => $itemAchado
	]);
	exit;
?>