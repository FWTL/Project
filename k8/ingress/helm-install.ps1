param(
    [Parameter(Mandatory=$True)]
    [System.String]
    $PathToChart
)

helm install "telegram-base" $PathToChart --namespace "telegram" --create-namespace
