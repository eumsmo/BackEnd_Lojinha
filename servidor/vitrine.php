<?php
	$quantidade = $_GET['quantidade'] ?? 0;

	if ($quantidade == 0) {
		header('Content-Type: application/json');
		echo json_encode([
			'status' => 'success',
			'itens' => [],
			'quantidade' => 0
		]);
		exit;
	}


	$json_content = file_get_contents('./itens.json');
	$itens = json_decode($json_content, true);
	$quant_itens = $itens['quantidade'];
	$chaves = array_rand($itens['itens'], $quantidade);
	$itens_selecionados = [];

	if ($quantidade > 1) {
		foreach ($chaves as $chave) {
			$item = $itens['itens'][$chave];
			unset($item['descricao']);
			$itens_selecionados[] = $item;
		}
	} else {
		$item = $itens['itens'][$chaves];
		unset($item['descricao']);
		$itens_selecionados[] = $item;
	}
	

	header('Content-Type: application/json');
	$json_responde = json_encode([
		'status' => 'success',
		'itens' => $itens_selecionados,
		'quantidade' => $quantidade
	]);
	echo $json_responde;
	exit;
?>