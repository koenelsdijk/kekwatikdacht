#!/bin/bash
JSON_DATA=$(cat "test.json")

# Use jq to parse and extract data
jq_output=$(echo "$JSON_DATA" | jq -r '
  .db[0].data.posts[] | [{
    filename: (if .status == "draft" then (.slug + "_DRAFT") else .slug end + ".html"),
    content: .html,
    type: .type
  }]
')

echo "$jq_output" | jq -c '.[]' | while read item; do
  filename=$(echo "$item" | jq -r '.filename')
  content=$(echo "$item" | jq -r '.content')
  dir=$(echo "$item" | jq -r '.type')s # <- 's' to create plural. Type can be "post" or "page".

  mkdir -p "$dir"
  echo "$content" > "$dir/$filename"
  echo "wrote to $dir/$filename"
done
