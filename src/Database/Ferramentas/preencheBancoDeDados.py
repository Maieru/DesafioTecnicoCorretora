import uuid
import random
from faker import Faker
from pathlib import Path
from datetime import datetime, timedelta

num_usuarios = 10000
num_ativos = 10000
operacoes_por_usuario = 30

faker = Faker("pt_BR")
Faker.seed(0)
random.seed(0)

usuarios_inserts = []
usuario_ids = []

# Usuários
for _ in range(num_usuarios):
    user_id = str(uuid.uuid4())
    usuario_ids.append(user_id)
    nome = faker.name().replace("'", "''")
    email = faker.unique.email().replace("'", "''")
    perc_corretagem = round(random.uniform(0.5, 5.0), 2)
    usuarios_inserts.append(
        f"INSERT INTO tbUsuarios (id, nome, email, perc_corretagem) VALUES ('{user_id}', '{nome}', '{email}', {perc_corretagem});"
    )

# Ativos e Cotações
ativos_inserts = []
cotacoes_inserts = []

for i in range(1, num_ativos + 1):
    codigo = f"{faker.random_uppercase_letter()}{faker.random_uppercase_letter()}{random.randint(1000,9999)}"
    nome = faker.company().replace("'", "''")
    ativos_inserts.append(
        f"INSERT INTO tbAtivos (codigo, nome) VALUES ('{codigo}', '{nome}');"
    )

    preco_unitario = round(random.uniform(5.00, 150.00), 2)
    data_hora = datetime.now().strftime('%Y-%m-%d %H:%M:%S')
    cotacoes_inserts.append(
        f"INSERT INTO tbCotacoes (ativo_id, preco_unitario, data_hora) VALUES ({i}, {preco_unitario}, '{data_hora}');"
    )

# Operações
operacoes_inserts = []

for usuario_id in usuario_ids:
    for _ in range(operacoes_por_usuario):
        ativo_id = random.randint(1, num_ativos)
        quantidade = random.randint(1, 500)
        preco_unitario = round(random.uniform(5.00, 150.00), 2)
        tipo_operacao = random.choice(['0', '1'])
        corretagem = round(random.uniform(0.5, 15.0), 2)
        dias_passados = random.randint(0, 90)
        data_hora = (datetime.now() - timedelta(days=dias_passados)).strftime('%Y-%m-%d %H:%M:%S')

        operacoes_inserts.append(
            f"INSERT INTO tbOperacoes (usuario_id, ativo_id, quantidade, preco_unitario, tipo_operacao, corretagem, data_hora) "
            f"VALUES ('{usuario_id}', {ativo_id}, {quantidade}, {preco_unitario}, '{tipo_operacao}', {corretagem}, '{data_hora}');"
        )

# Combinar tudo
script_sql_completo = "\n".join(usuarios_inserts + ativos_inserts + cotacoes_inserts + operacoes_inserts)

# Salvar em arquivo
output_path = Path("popular_todas_tabelas_com_cotacoes.sql")
output_path.write_text(script_sql_completo, encoding="utf-8")
