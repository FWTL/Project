param(
    [Parameter(Mandatory=$True)]
    [System.String]
    $PathToChart
)

helm install "Telegram" $PathToChart --create-namespace "Telegram"
